import { Component, OnInit, Input } from "@angular/core";
import { ActivatedRoute, Router, RouterLinkActive, RouterLink, RouterOutlet } from "@angular/router";
import { routeParams } from "src/app/app.routes";
import { AllocationPlanMinimalDto } from "../../generated/model/models";
import { Observable } from "rxjs";
import { ButtonGroupComponent } from "../button-group/button-group.component";
import { NgIf, NgFor, AsyncPipe } from "@angular/common";
import { PublicService } from "../../generated/api/public.service";

@Component({
    selector: "allocation-plan-select",
    templateUrl: "./allocation-plan-select.component.html",
    styleUrls: ["./allocation-plan-select.component.scss"],
    standalone: true,
    imports: [NgIf, ButtonGroupComponent, NgFor, RouterLinkActive, RouterLink, RouterOutlet, AsyncPipe],
})
export class AllocationPlanSelectComponent implements OnInit {
    @Input() geographyID: number;
    @Input() allocationPlans: AllocationPlanMinimalDto[];
    @Input() zoneGroupName: string = "Zone";
    @Input() showZoneToggle: boolean = true;

    public availableAllocationPlans: AllocationPlanMinimalDto[];
    public waterTypes: WaterTypeNameWithSlug[];
    public selectedWaterType: WaterTypeNameWithSlug;

    public allocationPlansDescription$: Observable<string>;

    constructor(private route: ActivatedRoute, private router: Router, private publicService: PublicService) {}

    ngOnInit(): void {
        this.allocationPlansDescription$ = this.publicService.publicGeographiesGeographyIDAllocationPlanConfigurationDescriptionGet(this.geographyID);

        // SMG: I hate doing this, but I am doing it.
        const addedWaterTypes = [];
        const uniqueTypes = [];
        this.allocationPlans.forEach((x) => {
            if (!addedWaterTypes.includes(x.WaterTypeSlug)) {
                uniqueTypes.push({ WaterTypeName: x.WaterTypeName, WaterTypeSlug: x.WaterTypeSlug } as WaterTypeNameWithSlug);
                addedWaterTypes.push(x.WaterTypeSlug);
            }
        });
        this.waterTypes = [...uniqueTypes];

        // getting funky in here!
        if (this.route.firstChild) {
            const waterTypeSlug = this.route.firstChild.snapshot.paramMap.get(routeParams.waterTypeSlug);
            const selectedWaterTypeIndex = this.waterTypes.findIndex((x) => x.WaterTypeSlug == waterTypeSlug);
            this.changeSelectedWaterType(this.waterTypes[selectedWaterTypeIndex]);
        } else {
            this.changeSelectedWaterType(this.waterTypes[0]);
        }

        // navigate to first zone, if available
        if (this.allocationPlans.length > 0) {
            this.router.navigate([this.selectedWaterType.WaterTypeSlug, this.allocationPlans[0].ZoneSlug], { relativeTo: this.route });
        }
    }

    changeSelectedWaterType(waterType: WaterTypeNameWithSlug) {
        this.selectedWaterType = waterType;
        this.availableAllocationPlans = this.allocationPlans.filter((x) => x.WaterTypeSlug == waterType.WaterTypeSlug);

        if (this.route.firstChild) {
            const zoneSlug = this.route.firstChild.snapshot.paramMap.get(routeParams.zoneSlug);
            this.router.navigate([waterType.WaterTypeSlug, zoneSlug], { relativeTo: this.route });
        }
    }
}

interface WaterTypeNameWithSlug {
    WaterTypeName: string;
    WaterTypeSlug: string;
}
