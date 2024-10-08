import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, IsActiveMatchOptions, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { filter, startWith, switchMap, tap } from 'rxjs/operators';
import { routeParams } from 'src/app/app.routes';
import { AuthenticationService } from 'src/app/shared/services/authentication.service';
import { GeographyService } from 'src/app/shared/generated/api/geography.service';
import { WaterAccountService } from 'src/app/shared/generated/api/water-account.service';
import { PermissionEnum } from 'src/app/shared/generated/enum/permission-enum';
import { GeographyDto } from 'src/app/shared/generated/model/geography-dto';
import { UserDto, WaterAccountMinimalDto } from 'src/app/shared/generated/model/models';
import { RightsEnum } from 'src/app/shared/models/enums/rights.enum';
import { LoadingDirective } from '../../../shared/directives/loading.directive';
import { FormsModule } from '@angular/forms';
import { SelectDropDownModule } from 'ngx-select-dropdown';
import { GeographyLogoComponent } from '../../../shared/components/geography-logo/geography-logo.component';
import { NgIf, AsyncPipe } from '@angular/common';
import { IconComponent } from 'src/app/shared/components/icon/icon.component';
import { DashboardMenu, DashboardMenuComponent } from 'src/app/shared/components/dashboard-menu/dashboard-menu.component';
import { PageHeaderComponent } from 'src/app/shared/components/page-header/page-header.component';
import { FlagEnum } from 'src/app/shared/generated/enum/flag-enum';

@Component({
  selector: 'water-account-detail-layout',
  templateUrl: './water-account-detail-layout.component.html',
  styleUrls: ['./water-account-detail-layout.component.scss'],
  standalone: true,
  imports: [
    NgIf,
    RouterLink,
    GeographyLogoComponent,
    IconComponent,
    SelectDropDownModule,
    FormsModule,
    RouterLinkActive,
    DashboardMenuComponent,
    RouterOutlet,
    PageHeaderComponent,
    LoadingDirective,
    AsyncPipe,
  ],
})
export class WaterAccountDetailLayoutComponent implements OnInit, OnDestroy {
  public waterAccountDropdownConfig = {
    search: true,
    height: '320px',
    placeholder: 'Water Account / Geography',
    searchOnKey: 'WaterAccountNameAndNumber',
  };

  private accountIDSubscription: Subscription = Subscription.EMPTY;
  public currentUser: UserDto;
  public currentUser$: Observable<UserDto>;
  public waterAccounts$: Observable<WaterAccountMinimalDto[]>;
  public navigationSubscription: Subscription = Subscription.EMPTY;
  public dashboardMenu: DashboardMenu;

  public waterAccounts: WaterAccountMinimalDto[];
  public filteredWaterAccounts: WaterAccountMinimalDto[];
  public currentWaterAccount: WaterAccountMinimalDto;

  public currentGeography$: Observable<GeographyDto>;

  public userHasOneGeography = false;

  constructor(
    private authenticationService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute,
    private geographyService: GeographyService,
    private waterAccountsService: WaterAccountService
  ) {}

  ngOnDestroy(): void {
    this.navigationSubscription.unsubscribe();
    this.accountIDSubscription.unsubscribe();
  }

  ngOnInit(): void {
    this.currentUser$ = this.authenticationService.getCurrentUser().pipe(
      tap((currentUser) => {
        this.currentUser = currentUser;

        // async pipe for waterAccounts
        this.waterAccounts$ = this.waterAccountsService.waterAccountsCurrentUserGet().pipe(
          tap((waterAccounts) => {
            this.waterAccounts = waterAccounts;
            const currentWaterAccountID = parseInt(this.route.snapshot.paramMap.get(routeParams.waterAccountID));
            this.filteredWaterAccounts = waterAccounts.filter((x) => x.WaterAccountID != currentWaterAccountID);
            this.navigationSubscription = this.getNavigationSubscription();
            this.currentWaterAccount = this.waterAccounts.find((x) => x.WaterAccountID == currentWaterAccountID);
            if (this.currentWaterAccount) {
              this.currentGeography$ = this.geographyService.geographiesGeographyIDGet(this.currentWaterAccount.Geography.GeographyID).pipe(
                tap((geography) => {
                  this.dashboardMenu = this.buildMenu(this.currentWaterAccount, geography);
                })
              );
            }
          })
        );
      })
    );
  }

  public viewingWaterAccount: boolean = false;

  private getNavigationSubscription(): Subscription {
    return this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        startWith(null as any),
        switchMap((e) => {
          if (this.route.firstChild) {
            return this.route.firstChild.paramMap;
          }
          return this.route.paramMap;
        })
      )
      .subscribe((paramMap) => {
        // do something on each navigation event
        const newWaterAccountID = parseInt(paramMap.get(routeParams.waterAccountID));
        this.viewingWaterAccount = !isNaN(newWaterAccountID) ? true : false;
        if (this.waterAccounts) {
          const selectedAccount = this.waterAccounts.find((x) => x.WaterAccountID == newWaterAccountID);
          this.currentWaterAccount = selectedAccount;
          if (this.currentWaterAccount) {
            this.currentGeography$ = this.geographyService.geographiesGeographyIDGet(this.currentWaterAccount.Geography.GeographyID).pipe(
              tap((geography) => {
                this.dashboardMenu = this.buildMenu(this.currentWaterAccount, geography);
              })
            );
          }
        }
      });
  }

  redirectToWaterAccount(waterAccountID: number) {
    if (waterAccountID) {
      this.router.navigateByUrl(`/water-dashboard/water-accounts/${waterAccountID}`, {});
    } else {
      this.router.navigateByUrl(`/water-dashboard`);
    }
  }

  changedWaterAccount(): void {
    this.redirectToWaterAccount(this.currentWaterAccount.WaterAccountID);
    this.filteredWaterAccounts = this.waterAccounts.filter((x) => x.WaterAccountID != this.currentWaterAccount.WaterAccountID);
  }

  isCurrentUserWaterAccountOwner() {
    if (!this.currentUser || !this.currentWaterAccount?.WaterAccountID) return false;

    const hasPermission = this.authenticationService.hasOverallPermission(
      this.currentUser,
      PermissionEnum.WaterAccountUserRights,
      RightsEnum.Create,
      this.currentWaterAccount?.Geography.GeographyID,
      this.currentWaterAccount.WaterAccountID
    );
    return hasPermission;
  }

  buildMenu(waterAccount: WaterAccountMinimalDto, geography: GeographyDto): DashboardMenu {
    const waterAccountID = waterAccount.WaterAccountID;
    const menu = {
      menuItems: [
        {
          title: `# ${waterAccount.WaterAccountNumber}`,
          icon: 'WaterAccounts',
          routerLink: ['/water-dashboard', 'water-accounts', waterAccountID],
          routerLinkActiveOptions: {
            matrixParams: 'ignored',
            queryParams: 'ignored',
            fragment: 'exact',
            paths: 'subset',
          },
          isDropdown: true,
          menuItems: [
            {
              title: 'Water Budget',
              routerLink: ['/water-dashboard', 'water-accounts', waterAccountID, 'water-budget'],
            },
            {
              title: 'Parcels',
              routerLink: ['/water-dashboard', 'water-accounts', waterAccountID, 'parcels'],
            },
            {
              title: 'Wells',
              routerLink: ['/water-dashboard', 'water-accounts', waterAccountID, 'wells'],
            },
            {
              title: 'Account Activity',
              routerLink: ['/water-dashboard', 'water-accounts', waterAccountID, 'activity'],
            },
            {
              title: 'Allocation Plans',
              routerLink: ['/water-dashboard', 'water-accounts', waterAccountID, 'allocation-plans'],
              routerLinkActiveOptions: {
                matrixParams: 'ignored',
                queryParams: 'ignored',
                fragment: 'exact',
                paths: 'subset',
              },
              isDisabled: !geography.AllocationPlansVisibleToLandowners,
            },
            {
              title: 'Users & Settings',
              routerLink: ['/water-dashboard', 'water-accounts', waterAccountID, 'users-and-settings'],
            },
            {
              title: 'Admin Panel',
              routerLink: ['/water-dashboard', 'water-accounts', waterAccountID, 'admin-panel'],
              isDisabled: !(
                this.authenticationService.isUserAnAdministrator(this.currentUser) ||
                                this.authenticationService.hasGeographyFlagForGeographyID(
                                  this.currentUser,
                                  FlagEnum.HasManagerDashboard,
                                  this.currentWaterAccount.Geography.GeographyID
                                )
              ),
            },
            {
              title: 'Back to All Water Accounts',
              icon: 'ArrowLeft',
              routerLink: ['/water-dashboard', 'water-accounts'],
              cssClasses: 'border-top',
            },
          ],
        },
        {
          title: 'Water Accounts',
          icon: 'WaterAccounts',
          routerLink: ['/water-dashboard/water-accounts'],
        },
        {
          title: 'Parcels',
          icon: 'Parcels',
          routerLink: ['/water-dashboard/parcels'],
        },
        {
          title: 'Wells',
          icon: 'Wells',
          routerLink: ['/water-dashboard/wells'],
        },

        {
          title: 'Support & Contact',
          icon: 'Question',
          routerLink: ['/geographies', this.currentWaterAccount.Geography.GeographyName.replace(' ', '-').toLowerCase(), 'support'],
        },
      ],
    } as DashboardMenu;
    menu.menuItems.forEach((menuItem) => {
      menuItem.menuItems?.forEach((childItem) => {
        const urltree = this.router.createUrlTree(childItem.routerLink as any[]);
        const childRouteIsActive = this.router.isActive(
          urltree,
          childItem.routerLinkActiveOptions
            ? childItem.routerLinkActiveOptions
            : ({ paths: 'exact', queryParams: 'ignored', matrixParams: 'ignored' } as IsActiveMatchOptions)
        );
        if (childRouteIsActive) {
          menuItem.isExpanded = true;
        }
      });
    });

    return menu;
  }
}
