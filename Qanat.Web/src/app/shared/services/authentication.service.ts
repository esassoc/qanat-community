import { Observable, race, ReplaySubject, Subject } from "rxjs";
import { concatMap, filter, first, map, takeUntil } from "rxjs/operators";
import { Router } from "@angular/router";
import { RoleEnum } from "../generated/enum/role-enum";
import { AlertService } from "./alert.service";
import { Alert } from "../models/alert";
import { AlertContext } from "../models/enums/alert-context.enum";
import { ImpersonationService } from "../generated/api/impersonation.service";
import { Inject, Injectable } from "@angular/core";
import { MsalBroadcastService, MsalGuardConfiguration, MsalService, MSAL_GUARD_CONFIG } from "@azure/msal-angular";
import { AuthenticationResult, EventMessage, EventType, InteractionStatus, InteractionType, PopupRequest, RedirectRequest } from "@azure/msal-browser";
import { b2cPolicies } from "../../auth.config";
import { UserClaimsService } from "../generated/api/user-claims.service";
import { UserDto } from "../generated/model/user-dto";
import { PermissionEnum } from "../generated/enum/permission-enum";
import { RightsEnum } from "../models/enums/rights.enum";
import { FlagEnum } from "../generated/enum/flag-enum";
import { GeographyDto } from "../generated/model/geography-dto";

@Injectable({
    providedIn: "root",
})
// todo: audit this class -> remove direct role references, move authorization functions elsewhere
export class AuthenticationService {
    private currentUser: UserDto;
    private claimsUser: any;
    private readonly _destroying$ = new Subject<void>();

    private _currentUserSetSubject = new ReplaySubject<UserDto>(1);
    public currentUserSetObservable = this._currentUserSetSubject.asObservable();

    constructor(
        private router: Router,
        @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
        private authService: MsalService,
        private msalBroadcastService: MsalBroadcastService,
        private userClaimsService: UserClaimsService,
        private impersonationService: ImpersonationService,
        private alertService: AlertService
    ) {
        // Required for MSAL to work: https://github.com/AzureAD/microsoft-authentication-library-for-js/issues/6719
        this.authService.handleRedirectObservable().subscribe();

        this.msalBroadcastService.msalSubject$
            .pipe(
                filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS),
                takeUntil(this._destroying$)
            )
            .subscribe((result: EventMessage) => {
                const payload = result.payload as AuthenticationResult;

                switch (payload.authority) {
                    case b2cPolicies.authorities.changeLogin.authority:
                    case b2cPolicies.authorities.editProfile.authority:
                        this.logout();
                        break;
                    case b2cPolicies.authorities.signUp.authority:
                        this.login(b2cPolicies.authorities.signUpSignIn as RedirectRequest);
                        break;
                    default:
                        this.authService.instance.setActiveAccount(payload.account);
                        this.claimsUser = this.authService.instance.getActiveAccount()?.idTokenClaims;
                        this.postUser();
                        break;
                }
            });

        this.msalBroadcastService.inProgress$.pipe(filter((status: InteractionStatus) => status === InteractionStatus.None)).subscribe(() => {
            this.checkAndSetActiveAccount();
            this.updateActiveAccount();
        });
    }

    checkAndSetActiveAccount() {
        /**
         * If no active account set but there are accounts signed in, sets first account to active account
         * To use active account set here, subscribe to inProgress$ first in your component
         * Note: Basic usage demonstrated. Your app may require more complicated account selection logic
         */
        const activeAccount = this.authService.instance.getActiveAccount();
        const allAccounts = this.authService.instance.getAllAccounts();

        if (!activeAccount && allAccounts.length > 0) {
            this.authService.instance.setActiveAccount(allAccounts[0]);
        }
    }

    updateActiveAccount(forceGet: boolean = false) {
        const newClaimsUser = this.authService.instance.getActiveAccount()?.idTokenClaims;
        const editProfilePolicy = b2cPolicies.names.editProfile.toLowerCase();
        if (newClaimsUser && newClaimsUser.acr == editProfilePolicy) {
            this.claimsUser = newClaimsUser;
            this.postUser();
        } else if (newClaimsUser && (!this.claimsUser || newClaimsUser.sub != this.claimsUser.sub || forceGet)) {
            this.claimsUser = newClaimsUser;
            this.getUser(this.claimsUser);
        }
    }

    private getUser(claims: any) {
        const globalID = claims.sub;

        this.userClaimsService.userClaimsGlobalIDGet(globalID).subscribe(
            (result) => {
                this.updateUser(result);
            },
            () => {
                this.onGetUserError();
            }
        );
    }

    private postUser() {
        this.userClaimsService.userClaimsPost().subscribe(
            (result) => {
                this.updateUser(result);
            },
            () => {
                this.onGetUserError();
            }
        );
    }

    private updateUser(user: UserDto) {
        this.currentUser = user;
        this._currentUserSetSubject.next(this.currentUser);
    }

    private onGetUserError() {
        this.router.navigate(["/"]).then((x) => {
            this.alertService.pushAlert(
                new Alert(
                    "There was an error authorizing with the application. The application will force log you out in 3 seconds, please try to login again.",
                    AlertContext.Danger
                )
            );
            setTimeout(() => {
                this.authService.logout();
            }, 3000);
        });
    }

    public refreshUserInfo(user: UserDto) {
        this.updateUser(user);
    }

    public isAuthenticated(): boolean {
        return this.claimsUser != null;
    }

    public handleUnauthorized(): void {
        this.forcedLogout();
    }

    public forcedLogout() {
        if (!this.isCurrentUserBeingImpersonated(this.currentUser)) {
            sessionStorage.authRedirectUrl = window.location.href;
        }
        this.logout();
    }

    public guardInitObservable(): Observable<AuthenticationResult> {
        return this.authService.initialize().pipe(
            concatMap(() => {
                return this.authService.handleRedirectObservable();
            })
        );
    }

    public login(userFlowRequest?: RedirectRequest | PopupRequest) {
        if (this.msalGuardConfig.interactionType === InteractionType.Popup) {
            if (this.msalGuardConfig.authRequest) {
                this.authService.loginPopup({ ...this.msalGuardConfig.authRequest, ...userFlowRequest } as PopupRequest).subscribe((response: AuthenticationResult) => {
                    this.authService.instance.setActiveAccount(response.account);
                });
            } else {
                this.authService.loginPopup(userFlowRequest).subscribe((response: AuthenticationResult) => {
                    this.authService.instance.setActiveAccount(response.account);
                });
            }
        } else {
            if (this.msalGuardConfig.authRequest) {
                this.authService.loginRedirect({ ...this.msalGuardConfig.authRequest, ...userFlowRequest } as RedirectRequest);
            } else {
                this.authService.loginRedirect(userFlowRequest);
            }
        }
    }

    public logout() {
        if (this.isCurrentUserBeingImpersonated(this.currentUser)) {
            this.impersonationService.impersonateStopImpersonationPost().subscribe((response) => {
                this.refreshUserInfo(response);
                this.router.navigateByUrl("/").then((x) => {
                    this.alertService.pushAlert(new Alert(`Finished impersonating`, AlertContext.Success));
                });
            });
        } else {
            this.authService.logout();
        }
    }

    resetPassword() {
        const resetPasswordRequest = {
            authority: b2cPolicies.authorities.passwordReset.authority,
            redirectUri: "/",
        } as RedirectRequest;

        this.login(resetPasswordRequest);
    }

    editProfile() {
        const editProfileFlowRequest = {
            authority: b2cPolicies.authorities.editProfile.authority,
            redirectUri: "/profile",

            // redirectStartPage: window.location.origin + '/profile'
        } as RedirectRequest;

        this.login(editProfileFlowRequest);
    }

    updateEmail() {
        const updateEmailRequest = {
            authority: b2cPolicies.authorities.changeLogin.authority,
            redirectUri: "/profile",
        } as RedirectRequest;

        this.login(updateEmailRequest);
    }

    signUp() {
        const signUpRequest = {
            scopes: ["openid"],
            authority: b2cPolicies.authorities.signUp.authority,
        } as RedirectRequest;

        this.login(signUpRequest);
    }

    public isCurrentUserBeingImpersonated(user: UserDto): boolean {
        if (user) {
            const globalID = this.claimsUser.sub;
            return globalID != user.UserGuid;
        }
        return false;
    }

    public getAuthRedirectUrl() {
        return sessionStorage.authRedirectUrl;
    }

    public setAuthRedirectUrl(url: string) {
        sessionStorage.authRedirectUrl = url;
    }

    public clearAuthRedirectUrl() {
        this.setAuthRedirectUrl("");
    }

    // todo: when clearing out these direct role references, do a global search for "RoleID =="

    public isUserUnassigned(user: UserDto): boolean {
        const role = user && user.Role ? user.Role.RoleID : null;
        return role === RoleEnum.NoAccess && user.IsActive;
    }

    public isUserRoleDisabled(user: UserDto): boolean {
        const role = user && user.Role ? user.Role.RoleID : null;
        return role === RoleEnum.NoAccess && !user.IsActive;
    }

    public isCurrentUserNullOrUndefined(): boolean {
        return !this.currentUser;
    }

    public isCurrentUserAnAdministrator(): boolean {
        return this.isUserAnAdministrator(this.currentUser);
    }

    public doesCurrentUserHaveOneOfTheseRoles(roleIDs: Array<number>): boolean {
        if (roleIDs.length === 0) {
            return false;
        }
        const roleID = this.currentUser && this.currentUser.Role ? this.currentUser.Role.RoleID : null;
        return roleIDs.includes(roleID);
    }

    public isUserAnAdministrator(user: UserDto): boolean {
        const role = user && user.Role ? user.Role.RoleID : null;
        return role === RoleEnum.SystemAdmin;
    }

    // todo: rights
    public isUserALandOwner(user: UserDto): boolean {
        const role = user && user.Role ? user.Role.RoleID : null;
        return role === RoleEnum.Normal;
    }

    public isCurrentUserALandOwner(): boolean {
        return this.isUserALandOwner(this.currentUser);
    }

    public getCurrentUser(): Observable<UserDto> {
        return race(
            new Observable((subscriber) => {
                if (this.currentUser) {
                    subscriber.next(this.currentUser);
                    subscriber.complete();
                }
            }),
            this.currentUserSetObservable.pipe(first())
        );
    }

    public getCurrentUserID(): Observable<any> {
        return race(
            new Observable((subscriber) => {
                if (this.currentUser) {
                    subscriber.next(this.currentUser.UserID);
                    subscriber.complete();
                }
            }),
            this.currentUserSetObservable.pipe(
                first(),
                map((user) => user.UserID)
            )
        );
    }

    // todo: move out of authentication service into authorization service (or something of the sort)

    public hasPermission(user: UserDto, permission: PermissionEnum, rights: RightsEnum): boolean {
        const permissionName = PermissionEnum[permission];

        const hasPermission = user && user.Rights && user.Rights[permissionName] ? user.Rights[permissionName][rights] : false;

        return hasPermission;
    }

    public hasOverallPermission(user: UserDto, permission: PermissionEnum, rights: RightsEnum, geographyID: number = null, waterAccountID: number = null): boolean {
        if (this.hasPermission(user, permission, rights)) {
            return true;
        }

        if (geographyID != null && this.hasGeographyPermission(user, permission, rights, geographyID)) {
            return true;
        }

        if (waterAccountID != null && this.hasWaterAccountPermission(user, permission, rights, waterAccountID)) {
            return true;
        }

        return false;
    }

    public hasGeographyPermission(user: UserDto, permission: PermissionEnum, rights: RightsEnum, geographyID: number): boolean {
        const hasRightsToGeography = Object.keys(user.GeographyRights).includes(geographyID.toString());
        if (!hasRightsToGeography) {
            return false;
        }

        const rightsToGeography = user.GeographyRights[geographyID.toString()];

        const permissionName = PermissionEnum[permission];
        const hasGeographyPermission = user && rightsToGeography[permissionName] ? rightsToGeography[permissionName][rights] : false;

        return hasGeographyPermission;
    }

    public hasWaterAccountPermission(user: UserDto, permission: PermissionEnum, rights: RightsEnum, waterAccountID: number): boolean {
        const hasRightsToWaterAccount = Object.keys(user.WaterAccountRights).includes(waterAccountID.toString());
        if (!hasRightsToWaterAccount) {
            return false;
        }

        const rightsToWaterAccount = user.WaterAccountRights[waterAccountID.toString()];

        const permissionName = PermissionEnum[permission];
        const hasWaterAccountPermission = user && rightsToWaterAccount[permissionName] ? rightsToWaterAccount[permissionName][rights] : false;

        return hasWaterAccountPermission;
    }

    public hasFlag(user: UserDto, flag: FlagEnum): boolean {
        const flagName = FlagEnum[flag];

        const hasFlag = user && user.Flags ? user.Flags[flagName] : false;

        return hasFlag;
    }

    public currentUserHasFlag(flag: FlagEnum) {
        return this.hasFlag(this.currentUser, flag);
    }

    public hasGeographyFlag(user: UserDto, flag: FlagEnum): boolean {
        const flagName = FlagEnum[flag];
        let flagFound = false;

        if (user) {
            Object.values(user.GeographyFlags).forEach((flags) => {
                if (flags[flagName]) {
                    flagFound = true;
                }
            });
        }

        return flagFound;
    }

    public hasGeographyFlagForGeographyID(user: UserDto, flag: FlagEnum, geographyID: number): boolean {
        const flagName = FlagEnum[flag];

        const hasFlag = user && user.GeographyFlags[geographyID] ? user.GeographyFlags[geographyID][flagName] : false;

        return hasFlag;
    }

    public hasWaterAccountFlag(user: UserDto, flag: FlagEnum): boolean {
        const flagName = FlagEnum[flag];
        let flagFound = false;

        if (user) {
            Object.values(user.WaterAccountFlags).forEach((flags) => {
                if (flags[flagName]) {
                    flagFound = true;
                }
            });
        }

        return flagFound;
    }

    public hasWaterAccountFlagForWaterAccountID(user: UserDto, flag: FlagEnum, waterAccountID: number): boolean {
        const flagName = FlagEnum[flag];

        const hasFlag = user && user.WaterAccountFlags[waterAccountID] ? user.WaterAccountFlags[waterAccountID][flagName] : false;

        return hasFlag;
    }

    public currentUserCanRequestWaterAccountChanges(geography: GeographyDto): boolean {
        if (this.isCurrentUserAnAdministrator()) {
            return true;
        }

        return geography.AllowLandownersToRequestAccountChanges;
    }

    ngOnDestroy(): void {
        this._destroying$.next(undefined);
        this._destroying$.complete();
    }
}
