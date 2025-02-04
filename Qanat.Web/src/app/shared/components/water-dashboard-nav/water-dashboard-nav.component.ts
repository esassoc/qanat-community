import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from "@angular/core";
import { RouterLink, RouterLinkActive } from "@angular/router";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DropdownToggleDirective } from "../../directives/dropdown-toggle.directive";
import { ModalService, ModalSizeEnum, ModalThemeEnum } from "../../services/modal/modal.service";
import { CreateWaterAccountComponent } from "../water-account/modals/create-water-account/create-water-account.component";
import { AsyncPipe, NgIf } from "@angular/common";
import { AuthenticationService } from "../../services/authentication.service";
import { Observable, tap } from "rxjs";
import { UserDto } from "../../generated/model/user-dto";
import { GeographyRoleEnum } from "../../generated/enum/geography-role-enum";
import { AuthorizationHelper } from "../../helpers/authorization-helper";
import { GeographyMinimalDto } from "../../generated/model/models";

@Component({
    selector: "water-dashboard-nav",
    standalone: true,
    imports: [RouterLink, RouterLinkActive, IconComponent, DropdownToggleDirective, NgIf, AsyncPipe],
    templateUrl: "./water-dashboard-nav.component.html",
    styleUrl: "./water-dashboard-nav.component.scss",
})
export class WaterDashboardNavComponent implements OnInit, OnChanges {
    @Input() geography: GeographyMinimalDto;
    @Output() waterAccountCreated = new EventEmitter();

    public user$: Observable<UserDto>;
    public canViewManageTools: boolean;
    public geographySlug: string;

    constructor(
        private modalService: ModalService,
        private authenticationService: AuthenticationService
    ) {}

    ngOnInit() {
        this.user$ = this.authenticationService.getCurrentUser().pipe(
            tap((user) => {
                this.canViewManageTools =
                    AuthorizationHelper.isSystemAdministrator(user) || user.GeographyUser.find((x) => x.GeographyRoleID == GeographyRoleEnum.WaterManager) != null;
            })
        );
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes && changes.geography && changes.geography.currentValue) {
            this.geographySlug = changes.geography.currentValue.GeographyName.toLowerCase();
        }
    }

    public createWaterAccountModal(): void {
        this.modalService
            .open(
                CreateWaterAccountComponent,
                null,
                { ModalSize: ModalSizeEnum.Medium, ModalTheme: ModalThemeEnum.Light, OverflowVisible: true },
                { geographyID: this.geography.GeographyID }
            )
            .instance.result.then((succeeded) => {
                this.waterAccountCreated.emit();
            });
    }
}
