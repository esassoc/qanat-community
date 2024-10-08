import { Component, OnDestroy, OnInit } from "@angular/core";
import { WaterTypeService } from "src/app/shared/generated/api/water-type.service";
import { WaterTypeSimpleDto } from "src/app/shared/generated/model/water-type-simple-dto";
import { CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";
import { Alert } from "src/app/shared/models/alert";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomWaterTypeDto } from "src/app/shared/components/water-types-configure/water-types-configure.component";
import { SelectedGeographyService } from "src/app/shared/services/selected-geography.service";
import { Subscription } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { RouterLink } from "@angular/router";
import { NgIf } from "@angular/common";
import { WaterTypesConfigureComponent } from "../../../shared/components/water-types-configure/water-types-configure.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";

@Component({
    selector: "water-supply-configure",
    templateUrl: "./water-supply-configure.component.html",
    styleUrls: ["./water-supply-configure.component.scss"],
    standalone: true,
    imports: [PageHeaderComponent, ModelNameTagComponent, AlertDisplayComponent, WaterTypesConfigureComponent, NgIf, RouterLink],
})
export class WaterSupplyConfigureComponent implements OnInit, OnDestroy {
    public geographyID: number;
    public currentUser: UserDto;

    public waterTypes: CustomWaterTypeDto[];
    public originalWaterTypes: string;
    public newWaterTypeName: string;
    public nextSortOrder: number;
    public isLoadingSubmit: boolean = false;

    public isEditing: boolean = false;
    public editedContent: string;
    public originalContent: string;

    public precipitationEnum = { OFF: "off", OPENET: "openet", CIMIS: "cimis", MANUAL: "manual" };
    public precipiationTab: string = this.precipitationEnum.OFF;
    public selectedGeography$ = Subscription.EMPTY;

    public hoverText = "This feature is necessary to the platform user experience and cannot be turned off.";

    constructor(
        private waterTypeService: WaterTypeService,
        private alertService: AlertService,
        private authenticationService: AuthenticationService,
        private selectedGeographyService: SelectedGeographyService
    ) {}

    ngOnInit(): void {
        this.authenticationService.getCurrentUser().subscribe((currentUser) => {
            this.currentUser = currentUser;
        });
        this.selectedGeography$ = this.selectedGeographyService.curentUserSelectedGeographyObservable.subscribe((geography) => {
            this.geographyID = geography.GeographyID;
            this.getDataForGeographyID(this.geographyID);
        });
    }

    getDataForGeographyID(geographyID: number) {
        this.waterTypeService.geographiesGeographyIDWaterTypesGet(geographyID).subscribe((waterTypes) => {
            this.updateWaterTypes(waterTypes);
        });
    }

    private updateWaterTypes(waterTypes: WaterTypeSimpleDto[]) {
        this.waterTypes = waterTypes.map((x) => new CustomWaterTypeDto(x.WaterTypeID, x.WaterTypeName, x.IsActive, x.WaterTypeDefinition));
        this.originalWaterTypes = JSON.stringify(this.waterTypes);
        this.nextSortOrder = this.waterTypes.length == 0 ? 1 : this.waterTypes[this.waterTypes.length - 1].SortOrder + 1;
    }

    saveWaterTypes() {
        const waterTypeSimpleDtos = this.waterTypes.map(
            (x, i) =>
                new WaterTypeSimpleDto({
                    WaterTypeID: x.WaterTypeID,
                    WaterTypeName: x.WaterTypeName,
                    WaterTypeDefinition: x.WaterTypeDefinition,
                    IsActive: x.IsActive,
                    SortOrder: i,
                })
        );

        this.waterTypeService.geographiesGeographyIDWaterTypeUpdatePost(this.geographyID, waterTypeSimpleDtos).subscribe((waterTypes) => {
            this.updateWaterTypes(waterTypes);
            this.alertService.pushAlert(new Alert(`Successfully saved!`, AlertContext.Success, true));
        });
    }

    drop(event: CdkDragDrop<string[]>) {
        moveItemInArray(this.waterTypes, event.previousIndex, event.currentIndex);
    }

    public cancelEdit(): void {
        this.isEditing = false;
    }

    canExit() {
        return (!this.isEditing || this.originalContent == this.editedContent) && this.originalWaterTypes == JSON.stringify(this.waterTypes);
    }

    ngOnDestroy(): void {
        this.selectedGeography$.unsubscribe();
    }
}
