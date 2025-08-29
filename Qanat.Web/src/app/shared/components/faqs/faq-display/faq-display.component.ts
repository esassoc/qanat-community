import { Component, Input, OnInit } from "@angular/core";
import { Observable, map } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { RolePermissionCheck, WithRolePermissionDirective } from "src/app/shared/directives/with-role-permission.directive";
import { FaqDisplayLocationTypeEnum } from "src/app/shared/generated/enum/faq-display-location-type-enum";
import { PermissionEnum } from "src/app/shared/generated/enum/permission-enum";
import { FrequentlyAskedQuestionSimpleDto } from "src/app/shared/generated/model/frequently-asked-question-simple-dto";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { RightsEnum } from "src/app/shared/models/enums/rights.enum";
import { FaqDisplayEditModalComponent } from "../faq-display-edit-modal/faq-display-edit-modal.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { FrequentlyAskedQuestionDisplayComponent } from "src/app/shared/components/frequently-asked-question-display/frequently-asked-question-display.component";
import { AsyncPipe } from "@angular/common";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "faq-display",
    templateUrl: "./faq-display.component.html",
    styleUrl: "./faq-display.component.scss",
    imports: [WithRolePermissionDirective, IconComponent, FrequentlyAskedQuestionDisplayComponent, WithRolePermissionDirective, AsyncPipe],
})
export class FaqDisplayComponent implements OnInit {
    faqs$: Observable<FrequentlyAskedQuestionSimpleDto[]>;
    currentUser$: Observable<UserDto>;
    rolePermissionCheck$: Observable<RolePermissionCheck>;
    @Input() faqDisplayLocationTypeID: FaqDisplayLocationTypeEnum;

    constructor(
        private authenticationService: AuthenticationService,
        private publicService: PublicService,
        private dialogService: DialogService
    ) {}

    ngOnInit(): void {
        this.rolePermissionCheck$ = this.authenticationService.getCurrentUser().pipe(
            map((currentUser) => {
                return {
                    permission: PermissionEnum.FrequentlyAskedQuestionRights,
                    rights: RightsEnum.Update,
                    currentUser,
                } as RolePermissionCheck;
            })
        );
        this.loadFAQs();
    }

    loadFAQs() {
        this.faqs$ = this.publicService.listFrequentlyAskedQuestionsByLocationTypeIDPublic(this.faqDisplayLocationTypeID);
    }

    public openEditModal() {
        const dialogRef = this.dialogService.open(FaqDisplayEditModalComponent, {
            data: {
                FrequentlyAskedQuestionID: null,
                FaqDisplayLocationTypeID: this.faqDisplayLocationTypeID,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.loadFAQs();
            }
        });
    }
}
