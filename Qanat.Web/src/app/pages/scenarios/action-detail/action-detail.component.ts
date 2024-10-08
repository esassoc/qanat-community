import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { GETActionService } from "src/app/shared/generated/api/get-action.service";
import { GETActionStatusEnum } from "src/app/shared/generated/enum/g-e-t-action-status-enum";
import { RoleEnum } from "src/app/shared/generated/enum/role-enum";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { GETActionDto } from "src/app/shared/generated/model/get-action-dto";
import { GetActionResult, UserDto } from "src/app/shared/generated/model/models";
import { ViewingGETActionService } from "src/app/shared/services/viewing-get-action.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { WithRoleDirective } from "../../../shared/directives/site-role-check.directive";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { NgIf, AsyncPipe } from "@angular/common";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { GetActionStatusBarComponent } from "src/app/shared/components/get-action-status-bar/get-action-status-bar.component";
import { TimeSeriesOutputComponent } from "src/app/shared/components/time-series-output/time-series-output.component";

@Component({
    selector: "action-detail",
    templateUrl: "./action-detail.component.html",
    styleUrls: ["./action-detail.component.scss"],
    standalone: true,
    imports: [
        LoadingDirective,
        NgIf,
        PageHeaderComponent,
        ModelNameTagComponent,
        AlertDisplayComponent,
        GetActionStatusBarComponent,
        WithRoleDirective,
        TimeSeriesOutputComponent,
        ButtonComponent,
        AsyncPipe,
    ],
})
export class ActionDetailComponent implements OnInit, OnDestroy {
    readonly RoleEnum = RoleEnum;
    readonly ScenarioEnum = ScenarioEnum;
    public currentUser$: Observable<UserDto>;
    public isLoadingSubmit: boolean = false;
    public getActionID: number;

    public GETActionStatusEnum = GETActionStatusEnum;
    public getAction$: Observable<GETActionDto>;
    public getActionResult$: Observable<GetActionResult>;

    public joinedOutput$: Observable<any>;

    public downloadError: boolean = false;
    public downloadErrorMessage: string;
    public isDownloading: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private getActionService: GETActionService,
        private cdr: ChangeDetectorRef,
        private authenticationService: AuthenticationService,
        private viewingGETActionService: ViewingGETActionService
    ) {}

    ngOnInit(): void {
        this.getActionID = parseInt(this.route.snapshot.paramMap.get(routeParams.actionID));
        this.getAction$ = this.getActionService.actionsGetActionIDGet(this.getActionID).pipe(
            tap((x) => {
                this.viewingGETActionService.loaded(x);
                // console.log(x);
            })
        );

        this.getActionResult$ = this.getActionService.actionsGetActionIDResultsGet(this.getActionID).pipe(
            tap((x) => {
                // console.log(x);
            })
        );

        this.currentUser$ = this.authenticationService.getCurrentUser();
    }

    checkStatus(): void {
        this.isLoadingSubmit = true;
        this.getAction$ = this.getActionService.actionsGetActionIDCheckStatusPost(this.getActionID).pipe(
            tap((x) => {
                this.isLoadingSubmit = false;
            })
        );
    }

    public downloadOutputJson() {
        this.downloadError = false;
        this.downloadErrorMessage = null;
        this.isDownloading = true;
        this.getActionService.actionsGetActionIDDownloadOutputJsonGet(this.getActionID).subscribe(
            (result) => this.handleDownloadSuccess(result, `Action_${this.getActionID}_Output.zip`, "application/zip"),
            (error) => this.handleDownloadError(error)
        );
    }

    private handleDownloadSuccess(result, fileName, contentType) {
        const blob = new Blob([result], {
            type: contentType,
        });

        //Create a fake object to trigger downloading the zip file that was returned
        const a: any = document.createElement("a");
        document.body.appendChild(a);

        a.style = "display: none";
        const url = window.URL.createObjectURL(blob);
        a.href = url;
        a.download = fileName;
        a.click();
        window.URL.revokeObjectURL(url);
        this.isDownloading = false;
    }

    private handleDownloadError(error) {
        this.downloadError = true;
        //Because our return type is ArrayBuffer, the message will be ugly. Convert it and display
        const decodedString = String.fromCharCode.apply(null, new Uint8Array(error.error) as any);
        this.downloadErrorMessage = decodedString;
        this.isDownloading = false;
    }

    ngOnDestroy() {
        this.cdr.detach();
        this.viewingGETActionService.unLoaded();
    }
}
