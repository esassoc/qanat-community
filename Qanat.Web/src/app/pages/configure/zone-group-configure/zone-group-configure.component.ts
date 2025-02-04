import { CdkDragDrop, moveItemInArray, CdkDropList, CdkDrag, CdkDragHandle } from "@angular/cdk/drag-drop";
import { Component, ComponentRef, OnInit, TemplateRef, ViewChild, ViewContainerRef } from "@angular/core";
import { map, Observable, switchMap, tap } from "rxjs";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { ZoneGroupMinimalDto, ZoneMinimalDto } from "src/app/shared/generated/model/models";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { AlertService } from "src/app/shared/services/alert.service";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { NoteComponent } from "../../../shared/components/note/note.component";
import { FormsModule } from "@angular/forms";
import { AsyncPipe, NgFor, NgIf } from "@angular/common";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { ActivatedRoute } from "@angular/router";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { routeParams } from "src/app/app.routes";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "zone-group-configure",
    templateUrl: "./zone-group-configure.component.html",
    styleUrls: ["./zone-group-configure.component.scss"],
    standalone: true,
    imports: [
        AsyncPipe,
        PageHeaderComponent,
        LoadingDirective,
        ModelNameTagComponent,
        AlertDisplayComponent,
        CdkDropList,
        NgFor,
        CdkDrag,
        NgIf,
        IconComponent,
        CdkDragHandle,
        FormsModule,
        NoteComponent,
    ],
})
export class ZoneGroupConfigureComponent implements OnInit {
    @ViewChild("deleteZoneGroupModal") deleteZoneGroupModal;

    public zoneGroups$: Observable<ZoneGroupMinimalDto[]>;
    public geographyID: number;
    public isLoading: boolean;

    private nextZoneGroupID: number = 0;

    public zonePrecipMultipliersEnabled: boolean;
    public isLoadingSubmit: boolean;
    public originalZoneGroupList: ZoneGroupMinimalDto[];
    public zoneGroupList: ZoneGroupMinimalDto[];
    public editingZoneGroup: ZoneGroupMinimalDto;
    public newZone: ZoneMinimalDto;
    private openModalComponent: ComponentRef<ModalComponent>;
    public richTextTypeID: number = CustomRichTextTypeEnum.ZoneGroupsEdit;

    public colorOrder = [
        { zoneColor: "#7F3C8D", zoneAccentColor: "#FFFFFF" },
        { zoneColor: "#11A579", zoneAccentColor: "#000000" },
        { zoneColor: "#3969AC", zoneAccentColor: "#FFFFFF" },
        { zoneColor: "#F2B701", zoneAccentColor: "#000000" },
        { zoneColor: "#E73F74", zoneAccentColor: "#000000" },
        { zoneColor: "#80BA5A", zoneAccentColor: "#000000" },
        { zoneColor: "#E68310", zoneAccentColor: "#000000" },
        { zoneColor: "#008695", zoneAccentColor: "#000000" },
        { zoneColor: "#CF1C90", zoneAccentColor: "#FFFFFF" },
        { zoneColor: "#f97b72", zoneAccentColor: "#000000" },
        { zoneColor: "#4b4b8f", zoneAccentColor: "#FFFFFF" },
        { zoneColor: "#A5AA99", zoneAccentColor: "#000000" },
    ];

    public currentColorIndex: number = null;
    public hoverText = "This feature is necessary to the platform user experience and cannot be turned off.";

    constructor(
        private route: ActivatedRoute,
        private zoneGroupService: ZoneGroupService,
        private alertService: AlertService,
        private modalService: ModalService,
        private viewContainerRef: ViewContainerRef,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService
    ) {}

    ngOnInit(): void {
        this.zoneGroups$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.geographiesGeographyNameGeographyNameMinimalGet(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;
            }),
            switchMap((geography) => {
                return this.zoneGroupService.geographiesGeographyIDZoneGroupsGet(geography.GeographyID);
            }),
            map((zoneGroups) => {
                return this.initializeZoneGroups(zoneGroups);
            }),
            tap(() => {
                this.isLoading = false;
            })
        );
    }

    private initializeZoneGroups(zoneGroups: ZoneGroupMinimalDto[]) {
        this.originalZoneGroupList = zoneGroups;
        this.zoneGroupList = zoneGroups.map(
            (x) =>
                new ZoneGroupMinimalDto({
                    ZoneGroupID: x.ZoneGroupID,
                    ZoneGroupName: x.ZoneGroupName,
                    ZoneGroupDescription: x.ZoneGroupDescription,
                    GeographyID: x.GeographyID,
                    HasAllocationPlan: x.HasAllocationPlan,
                    ZoneList: x.ZoneList.map(
                        (y) =>
                            new ZoneMinimalDto({
                                ZoneID: y.ZoneID,
                                ZoneName: y.ZoneName,
                                ZoneDescription: y.ZoneDescription,
                                ZoneColor: y.ZoneColor,
                                ZoneAccentColor: y.ZoneAccentColor,
                                PrecipMultiplier: y.PrecipMultiplier,
                                ZoneGroupID: y.ZoneGroupID,
                            })
                    ),
                    SortOrder: x.SortOrder,
                })
        );
        this.isLoadingSubmit = false;
        this.editingZoneGroup = null;
        this.currentColorIndex = 0;
        return this.zoneGroupList;
    }

    editZoneGroup(zoneGroup: ZoneGroupMinimalDto) {
        this.alertService.clearAlerts();
        this.editingZoneGroup = zoneGroup;
        this.initializeColorIndex();
        this.createNewZone();
    }

    initializeColorIndex() {
        if (this.currentColorIndex == null) this.currentColorIndex = 0;
        if (this.editingZoneGroup) {
            const lastColor = this.editingZoneGroup.ZoneList[this.editingZoneGroup.ZoneList.length - 1].ZoneColor;
            const indexOfLastColorUsed = this.colorOrder.findIndex((x) => lastColor == x.zoneColor);
            if (indexOfLastColorUsed != -1 && this.colorOrder.length > indexOfLastColorUsed + 1) {
                this.currentColorIndex = indexOfLastColorUsed + 1;
            } else {
                this.currentColorIndex = 0;
            }
        }
    }

    nextZoneColor() {
        if (this.currentColorIndex == null) {
            this.currentColorIndex = 0;
        }
        const color = this.colorOrder[this.currentColorIndex];
        if (this.currentColorIndex + 1 > this.colorOrder.length) {
            this.currentColorIndex = 0;
        } else {
            this.currentColorIndex++;
        }

        return color;
    }

    closeEdit() {
        this.initializeZoneGroups(this.originalZoneGroupList);
        this.alertService.clearAlerts();
    }

    addZoneGroup() {
        const zoneGroup = new ZoneGroupMinimalDto();
        zoneGroup.ZoneGroupID = this.nextZoneGroupID--;
        zoneGroup.GeographyID = this.geographyID;
        zoneGroup.SortOrder = this.zoneGroupList.length;
        zoneGroup.ZoneGroupName = this.newZoneGroupName();
        zoneGroup.ZoneGroupDescription = "Description";
        zoneGroup.ZoneList = this.addZoneList();

        this.zoneGroupList.push(zoneGroup);
        this.editZoneGroup(zoneGroup);
    }

    addZoneList() {
        const zone1 = new ZoneMinimalDto();
        const zone2 = new ZoneMinimalDto();
        const zone3 = new ZoneMinimalDto();

        let zoneColor = this.nextZoneColor();

        zone1.ZoneName = "Zone 1";
        zone1.ZoneDescription = "Zone";
        zone1.ZoneColor = zoneColor.zoneColor;
        zone1.ZoneAccentColor = zoneColor.zoneAccentColor;

        zoneColor = this.nextZoneColor();

        zone2.ZoneName = "Zone 2";
        zone2.ZoneDescription = "Zone";
        zone2.ZoneColor = zoneColor.zoneColor;
        zone2.ZoneAccentColor = zoneColor.zoneAccentColor;

        zoneColor = this.nextZoneColor();

        zone3.ZoneName = "Zone 3";
        zone3.ZoneDescription = "Zone";
        zone3.ZoneColor = zoneColor.zoneColor;
        zone3.ZoneAccentColor = zoneColor.zoneAccentColor;

        return [zone1, zone2, zone3];
    }

    addZone() {
        let validZone = true;

        if (this.newZone.ZoneName == null || this.newZone.ZoneName == "") {
            this.alertService.pushAlert(new Alert("Zone Name is a required field.", AlertContext.Danger));
            validZone = false;
        }
        if (this.editingZoneGroup.HasAllocationPlan && this.zonePrecipMultipliersEnabled && !this.newZone.PrecipMultiplier) {
            this.alertService.pushAlert(new Alert("Precip Multiplier is a required field.", AlertContext.Danger));
            validZone = false;
        }
        if (!validZone) {
            return;
        }

        this.editingZoneGroup.ZoneList.push(this.newZone);
        this.createNewZone();
    }

    removeZone(zone: ZoneMinimalDto) {
        const index = this.editingZoneGroup.ZoneList.indexOf(zone);
        this.editingZoneGroup.ZoneList.splice(index, index);
    }

    saveZoneGroup() {
        this.alertService.clearAlerts();
        const temp = this.editingZoneGroup.ZoneList.map(
            (x, i) =>
                new ZoneMinimalDto({
                    ZoneID: x.ZoneID,
                    ZoneName: x.ZoneName,
                    ZoneDescription: x.ZoneDescription,
                    ZoneColor: x.ZoneColor,
                    ZoneAccentColor: x.ZoneAccentColor,
                    ZoneGroupID: this.editingZoneGroup.ZoneGroupID,
                    PrecipMultiplier: x.PrecipMultiplier,
                    SortOrder: i,
                })
        );
        this.editingZoneGroup.ZoneList = temp;
        this.zoneGroupService.geographiesGeographyIDZoneGroupsPost(this.geographyID, this.editingZoneGroup).subscribe((zoneGroups) => {
            this.initializeZoneGroups(zoneGroups);
            this.alertService.pushAlert(new Alert("Successfully saved Zone Group.", AlertContext.Success, true));
        });
    }

    deleteZoneGroup() {
        this.isLoadingSubmit = true;
        this.zoneGroupService.geographiesGeographyIDZoneGroupZoneGroupIDDelete(this.geographyID, this.editingZoneGroup.ZoneGroupID).subscribe((zoneGroups) => {
            this.close();
            this.initializeZoneGroups(zoneGroups);
            this.alertService.pushAlert(new Alert("Successfully deleted Zone Group.", AlertContext.Success, true));
        });
    }

    getZonesText(zoneGroupDto: ZoneGroupMinimalDto) {
        let text = `<b>${zoneGroupDto.ZoneList.length} Zone${zoneGroupDto.ZoneList.length > 0 ? "s" : ""}: </b>`;
        text += zoneGroupDto.ZoneList.map((x) => x.ZoneName).join(", ");
        return text;
    }

    drop(event: CdkDragDrop<ZoneMinimalDto[]>) {
        moveItemInArray(this.editingZoneGroup.ZoneList, event.previousIndex, event.currentIndex);
    }

    dropZoneGroup(event: CdkDragDrop<string[]>) {
        moveItemInArray(this.zoneGroupList, event.previousIndex, event.currentIndex);
        const temp = this.zoneGroupList.map(
            (x, i) =>
                new ZoneGroupMinimalDto({
                    ZoneGroupID: x.ZoneGroupID,
                    GeographyID: x.GeographyID,
                    ZoneGroupName: x.ZoneGroupName,
                    ZoneGroupDescription: x.ZoneGroupDescription,
                    SortOrder: i,
                })
        );
        this.zoneGroupService.geographiesGeographyIDZoneGroupsSortOrderPut(this.geographyID, temp).subscribe((response) => {});
    }

    open(template: TemplateRef<any>): void {
        this.openModalComponent = this.modalService.open(template, this.viewContainerRef);
    }

    close(): void {
        if (!this.openModalComponent) return;
        this.modalService.close(this.openModalComponent);
    }

    newZoneGroupName(): string {
        const startName = "Zone Group";
        let newNumber = this.zoneGroupList.length;
        const existingNames = this.zoneGroupList.map((x) => x.ZoneGroupName);

        while (existingNames.includes(`${startName} ${newNumber}`)) {
            newNumber++;
        }

        return `${startName} ${newNumber}`;
    }

    canExit() {
        return (
            this.editingZoneGroup == null ||
            JSON.stringify(this.editingZoneGroup) == JSON.stringify(this.originalZoneGroupList.filter((x) => x.ZoneGroupID == this.editingZoneGroup.ZoneGroupID)[0])
        );
    }

    private createNewZone() {
        const zoneColor = this.nextZoneColor();

        this.newZone = new ZoneMinimalDto();
        this.newZone.ZoneColor = zoneColor.zoneColor;
        this.newZone.ZoneAccentColor = zoneColor.zoneAccentColor;
    }
}
