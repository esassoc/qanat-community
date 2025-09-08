import { Component, OnDestroy, OnInit } from "@angular/core";
import { WaterTypeSimpleDto } from "src/app/shared/generated/model/water-type-simple-dto";
import { CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";
import { Alert } from "src/app/shared/models/alert";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { CustomWaterTypeDto } from "src/app/shared/components/water-types-configure/water-types-configure.component";
import { map, Observable, switchMap, tap } from "rxjs";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { UserDto } from "src/app/shared/generated/model/user-dto";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { AsyncPipe } from "@angular/common";
import { WaterTypesConfigureComponent } from "../../../shared/components/water-types-configure/water-types-configure.component";
import { AlertDisplayComponent } from "../../../shared/components/alert-display/alert-display.component";
import { ModelNameTagComponent } from "../../../shared/components/name-tag/name-tag.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WaterTypeByGeographyService } from "src/app/shared/generated/api/water-type-by-geography.service";
import { CurrentGeographyService } from "src/app/shared/services/current-geography.service";
import { GeographyMinimalDto } from "src/app/shared/generated/model/models";
import { GeographyService } from "src/app/shared/generated/api/geography.service";
import { routeParams } from "src/app/app.routes";
import { LoadingDirective } from "src/app/shared/directives/loading.directive";

@Component({
    selector: "water-supply-configure",
    templateUrl: "./water-supply-configure.component.html",
    styleUrls: ["./water-supply-configure.component.scss"],
    imports: [AsyncPipe, LoadingDirective, PageHeaderComponent, ModelNameTagComponent, AlertDisplayComponent, WaterTypesConfigureComponent, RouterLink]
})
export class WaterSupplyConfigureComponent implements OnInit {
    public currentUser: UserDto;

    public waterTypes: CustomWaterTypeDto[];

    public waterTypes$: Observable<WaterTypeSimpleDto[]>;
    public geographyID: number;
    public isLoading: boolean;

    public originalWaterTypes: string;
    public newWaterTypeName: string;
    public nextSortOrder: number;
    public isLoadingSubmit: boolean = false;

    public isEditing: boolean = false;
    public editedContent: string;
    public originalContent: string;

    public precipitationEnum = { OFF: "off", OPENET: "openet", CIMIS: "cimis", MANUAL: "manual" };
    public precipiationTab: string = this.precipitationEnum.OFF;
    public geography$: Observable<GeographyMinimalDto>;

    public hoverText = "This feature is necessary to the platform user experience and cannot be turned off.";

    constructor(
        private route: ActivatedRoute,
        private waterTypeByGeographyService: WaterTypeByGeographyService,
        private alertService: AlertService,
        private currentGeographyService: CurrentGeographyService,
        private geographyService: GeographyService
    ) {}

    ngOnInit(): void {
        this.waterTypes$ = this.route.params.pipe(
            tap(() => {
                this.isLoading = true;
            }),
            switchMap((params) => {
                const geographyName = params[routeParams.geographyName];
                return this.geographyService.getByNameAsMinimalDtoGeography(geographyName);
            }),
            tap((geography) => {
                this.currentGeographyService.setCurrentGeography(geography);
                this.geographyID = geography.GeographyID;
            }),
            switchMap((geography) => {
                return this.waterTypeByGeographyService.getWaterTypesWaterTypeByGeography(geography.GeographyID);
            }),
            map((waterTypes) => {
                return waterTypes.map((wt) => new CustomWaterTypeDto(wt.WaterTypeID, wt.WaterTypeName, wt.IsActive, wt.WaterTypeColor, wt.WaterTypeDefinition));
            }),
            tap((waterTypes: CustomWaterTypeDto[]) => {
                this.updateWaterTypes(waterTypes);
                this.isLoading = false;
            })
        );
    }

    private updateWaterTypes(waterTypes: WaterTypeSimpleDto[]) {
        this.waterTypes = waterTypes.map((x) => new CustomWaterTypeDto(x.WaterTypeID, x.WaterTypeName, x.IsActive, x.WaterTypeColor, x.WaterTypeDefinition));
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
                    WaterTypeColor: x.WaterTypeColor,
                })
        );

        this.waterTypeByGeographyService.updateWaterTypesWaterTypeByGeography(this.geographyID, waterTypeSimpleDtos).subscribe((waterTypes) => {
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
}
