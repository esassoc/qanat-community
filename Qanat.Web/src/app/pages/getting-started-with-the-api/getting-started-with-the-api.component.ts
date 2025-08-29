import { Component, OnInit } from "@angular/core";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { KeyValuePairListComponent } from "src/app/shared/components/key-value-pair-list/key-value-pair-list.component";
import { KeyValuePairComponent } from "src/app/shared/components/key-value-pair/key-value-pair.component";
import { CopyToClipboardDirective } from "src/app/shared/directives/copy-to-clipboard.directive";
import { UserService } from "src/app/shared/generated/api/user.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { filter, Observable, of, share, switchMap } from "rxjs";
import { UserDto } from "src/app/shared/generated/model/models";
import { AsyncPipe } from "@angular/common";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";

@Component({
    selector: "getting-started-with-the-api",
    imports: [IconComponent, CustomRichTextComponent, NoteComponent, KeyValuePairListComponent, KeyValuePairComponent, AsyncPipe, CopyToClipboardDirective, AlertDisplayComponent],
    templateUrl: "./getting-started-with-the-api.component.html",
    styleUrl: "./getting-started-with-the-api.component.scss",
})
export class GettingStartedWithTheApiComponent implements OnInit {
    public APIKeyCustomRichTextTypeID: number = CustomRichTextTypeEnum.APIKey;
    public GettingStartedWithTheAPICustomRichTextTypeID: number = CustomRichTextTypeEnum.GettingStartedWithTheAPI;

    public currentUser$: Observable<UserDto>;
    public apiKey$: Observable<string>;
    public showAPIKey: boolean = false;

    constructor(
        private userService: UserService,
        private authenticationService: AuthenticationService,
        private confirmService: ConfirmService,
        private alertService: AlertService
    ) {}

    ngOnInit(): void {
        this.currentUser$ = this.authenticationService.getCurrentUser().pipe(share());
        this.apiKey$ = this.currentUser$.pipe(
            filter((user) => !!user),
            switchMap((user) => this.userService.getApiKeyUser(user.UserID))
        );
    }
    generateNewApiKeyModal(user: UserDto) {
        this.confirmService
            .confirm({
                title: "Generate New API Key",
                message: "Are you sure you want to generate a new API key? This will invalidate your current API key.",
                buttonTextYes: "Confirm",
                buttonTextNo: "Cancel",
                buttonClassYes: "btn-primary",
            })
            .then((result) => {
                if (result) {
                    this.userService.generateNewApiKeyUser(user.UserID).subscribe((response) => {
                        this.apiKey$ = of(response);
                        this.showAPIKey = true;
                        this.alertService.clearAlerts();
                        this.alertService.pushAlert(new Alert("New API Key generated successfully.", AlertContext.Success));
                    });
                }
            });
    }
}
