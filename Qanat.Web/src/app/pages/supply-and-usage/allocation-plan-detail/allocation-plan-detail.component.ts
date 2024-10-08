import { Component, OnDestroy, OnInit, ViewContainerRef } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Observable, Subscription } from "rxjs";
import { filter, switchMap, tap } from "rxjs/operators";
import { AllocationPlanManageDto, AllocationPlanMinimalDto } from "src/app/shared/generated/model/models";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { WaterAccountService } from "src/app/shared/generated/api/water-account.service";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { NgIf, AsyncPipe, DatePipe } from "@angular/common";
import { AllocationPlanTableComponent } from "src/app/shared/components/allocation-plan-table/allocation-plan-table.component";
import { routeParams } from "src/app/app.routes";
import {
    CopiedAllocationPlanEvent,
    CopyExistingAllocationPlanModalComponent,
} from "src/app/shared/components/copy-existing-allocation-plan-modal/copy-existing-allocation-plan-modal.component";
import { DeletedAllocationPeriodEvent } from "src/app/shared/components/delete-allocation-period-modal/delete-allocation-period-modal.component";
import {
    UpsertAllocationPeriodEvent,
    UpsertAllocationPeriodModalComponent,
    AllocationPeriodContext,
} from "src/app/shared/components/upsert-allocation-period-modal/upsert-allocation-period-modal.component";
import { TimeAgoPipe } from "src/app/shared/pipes/time-ago.pipe";
import { ModalService, ModalEvent, ModalSizeEnum, ModalThemeEnum, ModalOptions } from "src/app/shared/services/modal/modal.service";

@Component({
    selector: "allocation-plan-detail",
    templateUrl: "./allocation-plan-detail.component.html",
    styleUrls: ["./allocation-plan-detail.component.scss"],
    standalone: true,
    imports: [NgIf, AllocationPlanTableComponent, AsyncPipe, DatePipe, TimeAgoPipe],
})
export class AllocationPlanDetailComponent implements OnInit, OnDestroy {
    public isLoading: boolean = true;
    public allocationPlan$: Observable<AllocationPlanManageDto>;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;
    private allocationPlan: AllocationPlanManageDto;
    public geographyID: number;

    public editing: boolean;
    public canCopyFromExisting: boolean;

    private modalEventSubscription: Subscription = Subscription.EMPTY;

    constructor(
        private route: ActivatedRoute,
        private allocationPlanService: AllocationPlanService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private waterAccountService: WaterAccountService,
        private geographyService: GeographyService
    ) {}

    ngOnDestroy(): void {
        this.modalEventSubscription.unsubscribe();
    }

    ngOnInit(): void {
        this.modalEventSubscription = this.modalService.modalEventObservable
            .pipe(filter((e): e is ModalEvent => e instanceof DeletedAllocationPeriodEvent || e instanceof UpsertAllocationPeriodEvent || e instanceof CopiedAllocationPlanEvent))
            .subscribe((event) => {
                // should only get delete and upsert events here
                this.getGeographyID();
            });
        this.getGeographyID();

        this.editing = this.route.snapshot.data.editable ?? false;
    }

    getGeographyID(): void {
        // retrieving geographyID based on route context (not beautiful but it works)
        const geographyName = this.route.snapshot.paramMap.get(routeParams.geographyName);
        const waterAccountID = this.route.snapshot.paramMap.get(routeParams.waterAccountID);

        let request;
        if (geographyName) request = this.geographyService.publicGeographyNameGeographyNameGet(geographyName);
        if (waterAccountID) request = this.waterAccountService.waterAccountsWaterAccountIDGet(parseInt(waterAccountID));

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
                this.allocationPlanService
                    .publicAllocationPlansGeographyIDWaterTypeSlugZoneSlugGet(this.geographyID, params[routeParams.waterTypeSlug], params[routeParams.zoneSlug])
                    .pipe(
                        tap(
                            (allocationPlan) =>
                                (this.allocationPlans$ = this.allocationPlanService.publicGeographyGeographyIDAllocationPlansGet(this.geographyID).pipe(
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
        this.modalService
            .open(
                UpsertAllocationPeriodModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.ExtraLarge,
                    ModalTheme: ModalThemeEnum.Light,
                } as ModalOptions,
                {
                    AllocationPlanManageDto: this.allocationPlan,
                    Update: false,
                } as AllocationPeriodContext
            )
            .instance.result.then((result) => {
                if (result) {
                    this.setupObservable();
                }
            });
    }

    copyFromExistingPeriod(): void {
        this.modalService
            .open(
                CopyExistingAllocationPlanModalComponent,
                this.viewContainerRef,
                {
                    ModalSize: ModalSizeEnum.Medium,
                    ModalTheme: ModalThemeEnum.Light,
                } as ModalOptions,
                {
                    AllocationPlanManageDto: this.allocationPlan,
                } as AllocationPeriodContext
            )
            .instance.result.then((result) => {
                if (result) {
                    this.setupObservable();
                }
            });
    }
}
