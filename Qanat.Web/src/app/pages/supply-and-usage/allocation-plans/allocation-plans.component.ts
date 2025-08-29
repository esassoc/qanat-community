import { Component, OnInit } from "@angular/core";
import { Observable } from "rxjs";
import { switchMap, tap } from "rxjs/operators";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { AllocationPlanMinimalDto, GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { AsyncPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { AllocationPlanSelectComponent } from "src/app/shared/components/allocation-plan-select/allocation-plan-select.component";
import { AlertDisplayComponent } from "src/app/shared/components/alert-display/alert-display.component";
import { PublicService } from "src/app/shared/generated/api/public.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";

@Component({
    selector: "allocation-plans",
    templateUrl: "./allocation-plans.component.html",
    styleUrls: ["./allocation-plans.component.scss"],
    imports: [PageHeaderComponent, AlertDisplayComponent, AllocationPlanSelectComponent, AsyncPipe]
})
export class AllocationPlansComponent implements OnInit {
    public customRichTextID = CustomRichTextTypeEnum.AllocationPlanEdit;
    public geography$: Observable<GeographyMinimalDto>;
    public allocationPlans$: Observable<AllocationPlanMinimalDto[]>;

    constructor(
        private currentGeographyService: CurrentGeographyService,
        private publicService: PublicService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();

        this.allocationPlans$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.publicService.listAllocationPlansByGeographyIDPublic(geography.GeographyID);
            })
        );
    }
}
