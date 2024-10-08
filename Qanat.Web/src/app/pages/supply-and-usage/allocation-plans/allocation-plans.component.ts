import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { AllocationPlanService } from "src/app/shared/generated/api/allocation-plan.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto } from "src/app/shared/generated/model/geography-dto";
import { AllocationPlanMinimalDto } from "src/app/shared/generated/model/models";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { NgIf, AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AllocationPlanSelectComponent } from "src/app/shared/components/allocation-plan-select/allocation-plan-select.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";

@Component({
    selector: "allocation-plans",
    templateUrl: "./allocation-plans.component.html",
    styleUrls: ["./allocation-plans.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, NgIf, AlertDisplayComponent, AllocationPlanSelectComponent, AsyncPipe],
})
export class AllocationPlansComponent implements OnInit {
    public customRichTextID = CustomRichTextTypeEnum.AllocationPlanEdit;
    public selectedGeography$: Observable<GeographyDto>;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;

    constructor(
        private selectedGeographyService: SelectedGeographyService,
        private allocationPlanService: AllocationPlanService
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.pipe(
            tap((geography) => (this.allocationPlans$ = this.allocationPlanService.publicGeographyGeographyIDAllocationPlansGet(geography.GeographyID)))
        );
    }
}
