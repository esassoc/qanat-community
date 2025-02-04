import { Component, OnInit } from "@angular/core";
import { ColDef } from "ag-grid-community";
import { Observable, switchMap, tap } from "rxjs";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { GeographyDto, ZoneGroupDto } from "src/app/shared/generated/model/models";
import { QanatMapComponent, QanatMapInitEvent } from "src/app/shared/components/leaflet/qanat-map/qanat-map.component";
import { RouterLink } from "@angular/router";
import { AlertDisplayComponent } from "../../../../shared/components/alert-display/alert-display.component";
import { NgIf, NgFor, AsyncPipe, DecimalPipe } from "@angular/common";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { ZoneGroupLayerComponent } from "src/app/shared/components/leaflet/layers/zone-group-layer/zone-group-layer.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "zone-group-list",
    templateUrl: "./zone-group-list.component.html",
    styleUrls: ["./zone-group-list.component.scss"],
    standalone: true,
    imports: [
        PageHeaderComponent,
        LoadingDirective,
        NgIf,
        AlertDisplayComponent,
        NgFor,
        IconComponent,
        RouterLink,
        QanatMapComponent,
        ZoneGroupLayerComponent,
        AsyncPipe,
        DecimalPipe,
    ],
})
export class ZoneGroupListComponent implements OnInit {
    public geography$: Observable<GeographyDto>;
    public zoneGroups$: Observable<ZoneGroupDto[]>;
    public isLoading: boolean = true;
    public columnDefs: Array<ColDef>;
    public richTextTypeID: number = CustomRichTextTypeEnum.ZoneGroupList;

    constructor(
        private zoneGroupService: ZoneGroupService,
        private currentGeographyService: CurrentGeographyService
    ) {}

    ngOnInit(): void {
        this.geography$ = this.currentGeographyService.getCurrentGeography();

        this.zoneGroups$ = this.geography$.pipe(
            switchMap((geography) => {
                return this.zoneGroupService.geographiesGeographyIDZoneGroupsDetailedGet(geography.GeographyID);
            }),
            tap(() => {
                this.isLoading = false;
            })
        );
    }

    public mapInits: { [key: number]: QanatMapInitEvent } = {};

    handleMapReady(event: QanatMapInitEvent, zoneGroup: ZoneGroupDto): void {
        this.mapInits[zoneGroup.ZoneGroupID] = event;
    }
}
