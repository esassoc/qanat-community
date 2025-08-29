import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Observable } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { routeParams } from "src/app/app.routes";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { ScenarioEnum } from "src/app/shared/generated/enum/scenario-enum";
import { ScenarioRunDto, ScenarioRunResult, UserDto } from "src/app/shared/generated/model/models";
import { CurrentScenarioRunService } from "src/app/shared/services/viewing-get-action.service";
import { ButtonComponent } from "../../../shared/components/button/button.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { AsyncPipe } from "@angular/common";
import { LoadingDirective } from "../../../shared/directives/loading.directive";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { ScenarioRunStatusBarComponent } from "src/app/shared/components/scenario-run-status-bar/scenario-run-status-bar.component";
import { TimeSeriesOutputComponent } from "src/app/shared/components/time-series-output/time-series-output.component";
import { WithFlagDirective } from "src/app/shared/directives/with-flag.directive";
import { FlagEnum } from "src/app/shared/generated/enum/flag-enum";
import { ScenarioRunService } from "src/app/shared/generated/api/scenario-run.service";
import { ScenarioRunStatusEnum } from "src/app/shared/generated/enum/scenario-run-status-enum";

@Component({
    selector: "scenario-run-detail",
    templateUrl: "./scenario-run-detail.component.html",
    styleUrls: ["./scenario-run-detail.component.scss"],
    imports: [
        LoadingDirective,
        PageHeaderComponent,
        ModelNameTagComponent,
        AlertDisplayComponent,
        ScenarioRunStatusBarComponent,
        WithFlagDirective,
        TimeSeriesOutputComponent,
        ButtonComponent,
        AsyncPipe,
    ]
})
export class ScenarioRunDetailComponent implements OnInit, OnDestroy {
    public readonly FlagEnum = FlagEnum;
    public readonly ScenarioEnum = ScenarioEnum;
    public readonly ScenarioRunStatusEnum = ScenarioRunStatusEnum;

    public isLoadingSubmit: boolean = false;

    public currentUser$: Observable<UserDto>;

    public scenarioRun$: Observable<ScenarioRunDto>;
    public scenarioRunID: number;

    public scenarioRunResult$: Observable<ScenarioRunResult>;

    public joinedOutput$: Observable<any>;

    public downloadError: boolean = false;
    public downloadErrorMessage: string;
    public isDownloading: boolean = false;

    constructor(
        private route: ActivatedRoute,
        private scenarioRunService: ScenarioRunService,
        private authenticationService: AuthenticationService,
        private currentScenarioRunService: CurrentScenarioRunService
    ) {}

    ngOnInit(): void {
        this.scenarioRun$ = this.route.params.pipe(
            tap((params) => {
                this.scenarioRunID = parseInt(params[routeParams.scenarioRunID]);
            }),
            switchMap(() =>
                this.scenarioRunService.getScenarioRunByIDScenarioRun(this.scenarioRunID).pipe(
                    tap((x) => {
                        this.currentScenarioRunService.loaded(x);
                    })
                )
            )
        );

        this.scenarioRunResult$ = this.scenarioRun$.pipe(
            switchMap((scenarioRun) => {
                return this.scenarioRunService.getBudgetGroundwaterOutputScenarioRun(scenarioRun.ScenarioRunID);
            })
        );

        this.currentUser$ = this.authenticationService.getCurrentUser();
    }

    ngOnDestroy() {
        this.currentScenarioRunService.unloaded();
    }

    checkStatus(): void {
        this.isLoadingSubmit = true;
        this.scenarioRun$ = this.scenarioRunService.checkScenarioRunStatusScenarioRun(this.scenarioRunID).pipe(
            tap((x) => {
                this.isLoadingSubmit = false;
            })
        );
    }

    public downloadOutputJson() {
        this.downloadError = false;
        this.downloadErrorMessage = null;
        this.isDownloading = true;
        this.scenarioRunService.downloadOutputJsonScenarioRun(this.scenarioRunID).subscribe(
            (result) => this.handleDownloadSuccess(result, `ScenarioRun_${this.scenarioRunID}_Output.zip`, "application/zip"),
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
}
