import { Component, OnDestroy, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { AllocationPlanManageDto, AllocationPlanMinimalDto } from "src/app/shared/generated/model/models";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { AsyncPipe, DatePipe } from "@angular/common";
import { AllocationPlanTableComponent } from "src/app/shared/components/allocation-plan-table/allocation-plan-table.component";
import { routeParams } from "src/app/app.routes";
import { CopyExistingAllocationPlanModalComponent } from "src/app/shared/components/copy-existing-allocation-plan-modal/copy-existing-allocation-plan-modal.component";
import { UpsertAllocationPeriodModalComponent } from "src/app/shared/components/upsert-allocation-period-modal/upsert-allocation-period-modal.component";
import { TimeAgoPipe } from "src/app/shared/pipes/time-ago.pipe";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { DialogService } from "@ngneat/dialog";

@Component({
    selector: "allocation-plan-detail",
    templateUrl: "./allocation-plan-detail.component.html",
    styleUrls: ["./allocation-plan-detail.component.scss"],
    imports: [AllocationPlanTableComponent, AsyncPipe, DatePipe, TimeAgoPipe],
})
export class AllocationPlanDetailComponent implements OnInit, OnDestroy {
    public isLoading: boolean = true;
    public allocationPlan$: Observable<AllocationPlanManageDto>;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;
    private allocationPlan: AllocationPlanManageDto;
    public geographyID: number;

    public editing: boolean;
    public canCopyFromExisting: boolean;

    constructor(
        private route: ActivatedRoute,
        private publicService: PublicService,
        private waterAccountService: WaterAccountService,
        private dialogService: DialogService
    ) {}

    ngOnDestroy(): void {}

    ngOnInit(): void {
        this.getGeographyID();

        this.editing = this.route.snapshot.data.editable ?? false;
    }

    getGeographyID(): void {
        // retrieving geographyID based on route context (not beautiful but it works)
        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        const waterAccountID = this.route.snapshot.paramMap.get(routeParams.waterAccountID);

        let request;
        if (geographyName) request = this.publicService.getGeographyByNamePublic(geographyName);
        if (waterAccountID) request = this.waterAccountService.getByIDWaterAccount(parseInt(waterAccountID));

        if (!request) return;

        request.subscribe((result) => {
            this.geographyID = result.Geography?.GeographyID ?? result.GeographyID;
            this.setupObservable();
        });
    }

    setupObservable(): void {
        this.allocationPlan$ = this.route.params.pipe(
            tap((params) => (this.isLoading = true)),
            switchMap((params) =>
                this.publicService.getAllocationPlanByWaterTypeSlugAndZoneSlugPublic(this.geographyID, params[routeParams.waterTypeSlug], params[routeParams.zoneSlug]).pipe(
                    tap(
                        (allocationPlan) =>
                            (this.allocationPlans$ = this.publicService.listAllocationPlansByGeographyIDPublic(this.geographyID).pipe(
                                tap((allocationPlans) => {
                                    this.canCopyFromExisting =
                                        allocationPlans.filter((x) => x.AllocationPlanID != allocationPlan.AllocationPlanID && x.AllocationPeriodsCount > 0).length > 0;
                                })
                            ))
                    )
                )
            ),
            tap((x) => {
                this.isLoading = false;
                this.allocationPlan = x;
            })
        );
    }

    addAllocationPeriod(): void {
        const dialogRef = this.dialogService.open(UpsertAllocationPeriodModalComponent, {
            data: {
                AllocationPlanPeriodSimpleDto: null,
                AllocationPlanManageDto: this.allocationPlan,
                Update: false,
            },
            size: "lg",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.setupObservable();
            }
        });
    }

    copyFromExistingPeriod(): void {
        const dialogRef = this.dialogService.open(CopyExistingAllocationPlanModalComponent, {
            data: {
                AllocationPlanPeriodSimpleDto: null,
                AllocationPlanManageDto: this.allocationPlan,
                Update: false,
            },
            size: "sm",
        });

        dialogRef.afterClosed$.subscribe((result) => {
            if (result) {
                this.setupObservable();
            }
        });
    }
}
