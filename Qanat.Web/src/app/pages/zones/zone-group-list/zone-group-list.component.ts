import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { Observable, Subscription } from "rxjs";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto, ZoneDetailedDto, ZoneGroupMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyRouteService } from "src/app/shared/services/geography-route.service";

import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { RouterLink } from "@angular/router";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { NgIf, NgFor, AsyncPipe, DecimalPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
@Component({
    selector: "zone-group-list",
    templateUrl: "./zone-group-list.component.html",
    styleUrls: ["./zone-group-list.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, NgIf, AlertDisplayComponent, NgFor, IconComponent, RouterLink, QanatMapComponent, ZoneGroupLayerComponent, AsyncPipe, DecimalPipe],
})
export class ZoneGroupListComponent implements OnInit {
    private selectedGeography$: Subscription = Subscription.EMPTY;
    public zoneGroupSubscription$: Observable<ZoneGroupMinimalDto[]>;
    public zones: ZoneDetailedDto[];
    public geographyID: number;
    public geography: GeographyDto;
    public isLoadingSubmit: boolean;
    public columnDefs: Array<ColDef>;
    public richTextTypeID: number = CustomRichTextTypeEnum.ZoneGroupList;

    constructor(
        private zoneGroupService: ZoneGroupService,
        private geographyRouteService: GeographyRouteService,
        private cdr: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        this.selectedGeography$ = this.geographyRouteService.geography$.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.geography = geography;
            this.zoneGroupService.geographiesGeographyIDZonesGet(this.geographyID).subscribe((zones) => {
                this.zones = zones;
            });
            this.zoneGroupSubscription$ = this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(this.geographyID).pipe();
        });
    }

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }

    public getTotalParcels(zoneID: number) {
        return this.zones.find((x) => x.ZoneID === zoneID)?.TotalParcels;
    }

    public getTotalArea(zoneID: number) {
        return this.zones.find((x) => x.ZoneID === zoneID)?.TotalArea;
    }

    public mapInits: { [key: number]: QanatMapInitEvent } = {};

    handleMapReady(event: QanatMapInitEvent, zoneGroup: ZoneGroupMinimalDto): void {
        this.mapInits[zoneGroup.ZoneGroupID] = event;
        this.cdr.detectChanges();
    }
}
