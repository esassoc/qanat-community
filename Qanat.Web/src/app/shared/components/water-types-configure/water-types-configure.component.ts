import { Component, Input, OnChanges, SimpleChanges } from "@angular/core";
import { WaterTypeSimpleDto } from "src/app/shared/generated/model/water-type-simple-dto";
import { CdkDragDrop, moveItemInArray, CdkDropList, CdkDrag, CdkDragHandle } from "@angular/cdk/drag-drop";
import { Alert } from "src/app/shared/models/alert";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { UserDto } from "../../generated/model/user-dto";
import { WaterTypeFieldDefinitionComponent } from "../water-type-field-definition/water-type-field-definition.component";
import { FormsModule } from "@angular/forms";

@Component({
    selector: "water-types-configure",
    templateUrl: "./water-types-configure.component.html",
    styleUrls: ["./water-types-configure.component.scss"],
    imports: [CdkDropList, CdkDrag, FormsModule, WaterTypeFieldDefinitionComponent, CdkDragHandle]
})
export class WaterTypesConfigureComponent implements OnChanges {
    @Input() geographyID: number;
    @Input() currentUser: UserDto;
    @Input() waterTypes: CustomWaterTypeDto[];
    @Input() isUsageConfiguration: boolean = false;
    public newWaterTypeName: string;
    public isLoadingSubmit: boolean = false;

    public colorOrder: string[] = ["#7F3C8D", "#11A579", "#3969AC", "#F2B701", "#E73F74", "#80BA5A", "#E68310", "#008695", "#CF1C90", "#f97b72", "#4b4b8f", "#A5AA99"];
    public currentColorIndex: number;

    constructor(private alertService: AlertService) {}

    ngOnChanges(changes: SimpleChanges): void {
        if (changes.waterTypes) {
            this.updateCurrentColorIndex();
        }
    }

    createNewWaterType() {
        const waterTypeNamingConflict = this.waterTypes.find((x) => x.WaterTypeName.toLowerCase() == this.newWaterTypeName.toLowerCase());
        if (waterTypeNamingConflict) {
            this.alertService.pushAlert(new Alert(`There is a water type already named: ${this.newWaterTypeName}`, AlertContext.Danger, true));
            return;
        }

        this.waterTypes.push(new CustomWaterTypeDto(0, this.newWaterTypeName, true, this.colorOrder[this.currentColorIndex]));
        this.newWaterTypeName = null;
        this.updateCurrentColorIndex();
    }

    updateCurrentColorIndex() {
        this.currentColorIndex = this.waterTypes ? this.waterTypes.length % this.colorOrder.length : 0;
    }

    toggleIsActive(waterType: WaterTypeSimpleDto) {
        waterType.IsActive = !waterType.IsActive;
    }

    drop(event: CdkDragDrop<string[]>) {
        moveItemInArray(this.waterTypes, event.previousIndex, event.currentIndex);
    }

    public getPlaceholder(): string {
        return `(e.g. ${this.isUsageConfiguration ? "Meter Data" : "Groundwater Allocation"})`;
    }
}

export class CustomWaterTypeDto {
    WaterTypeID: number;
    WaterTypeName: string;
    WaterTypeDefinition: string;
    IsActive: boolean;
    SortOrder: number;
    WaterTypeColor: string;

    constructor(waterTypeID: number, waterTypeName: string, isActive: boolean, waterTypeColor: string, waterTypeDefinition?: string) {
        this.WaterTypeID = waterTypeID;
        this.WaterTypeName = waterTypeName;
        this.IsActive = isActive;
        this.WaterTypeDefinition = waterTypeDefinition;
        this.WaterTypeColor = waterTypeColor;
    }
}
