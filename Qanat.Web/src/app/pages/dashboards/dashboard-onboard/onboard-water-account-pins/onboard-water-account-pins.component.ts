import { Component, OnInit } from "@angular/core";
import { NgForm, FormsModule } from "@angular/forms";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { WaterAccountUserService } from "src/app/shared/generated/api/water-account-user.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { OnboardWaterAccountPINDto } from "src/app/shared/generated/model/onboard-water-account-pin-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { CustomRichTextComponent } from "../../../../shared/components/custom-rich-text/custom-rich-text.component";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { FieldDefinitionComponent } from "../../../../shared/components/field-definition/field-definition.component";
import { ButtonComponent } from "../../../../shared/components/button/button.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { PublicService } from "src/app/shared/generated/api/public.service";

@Component({
    selector: "onboard-water-account-pins",
    templateUrl: "./onboard-water-account-pins.component.html",
    styleUrls: ["./onboard-water-account-pins.component.scss"],
    standalone: true,
    imports: [NgIf, PageHeaderComponent, FormsModule, ButtonComponent, FieldDefinitionComponent, AlertDisplayComponent, NgFor, CustomRichTextComponent, RouterLink, AsyncPipe],
})
export class OnboardWaterAccountPINsComponent implements OnInit {
    public currentUser$: Observable<UserDto>;
    public geography$: Observable<GeographyDto>;
    public claimedWaterAccountPINs$: Observable<OnboardWaterAccountPINDto[]>;

    public geographyID: number;
    public customRichTextID: CustomRichTextTypeEnum = CustomRichTextTypeEnum.OnboardWaterAccountPINs;
    public claimedWaterAccountPINs: OnboardWaterAccountPINDto[];
    public waterAccountPINToClaim: string;

    constructor(
        private authenticationService: AuthenticationService,
        private waterAccountUserService: WaterAccountUserService,
        private alertService: AlertService,
        private route: ActivatedRoute,
        private publicService: PublicService
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser();

        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        this.geography$ = this.publicService.publicGeographiesNameGeographyNameGet(geographyName).pipe(
            tap((geography) => {
                this.geographyID = geography.GeographyID;
                this.claimedWaterAccountPINs$ = this.waterAccountUserService.geographiesGeographyIDWaterAccountPINsGet(this.geographyID);
            })
        );
    }

    isAuthenticated(): boolean {
        return this.authenticationService.isAuthenticated();
    }

    onSubmit(form: NgForm): void {
        this.waterAccountUserService.geographiesGeographyIDWaterAccountWaterAccountPINWaterAccountPINPost(this.geographyID, this.waterAccountPINToClaim).subscribe((response) => {
            this.claimedWaterAccountPINs$ = this.waterAccountUserService.geographiesGeographyIDWaterAccountPINsGet(this.geographyID);
            this.waterAccountPINToClaim = "";
            this.alertService.clearAlerts();
            this.alertService.pushAlert(new Alert("Water Acount PIN successfully saved.", AlertContext.Success, true));
            form.reset();
        });
    }
}
