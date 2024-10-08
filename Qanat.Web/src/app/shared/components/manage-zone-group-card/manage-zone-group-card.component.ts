import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ZoneGroupMinimalDto } from "src/app/shared/generated/model/zone-group-minimal-dto";
import saveAs from "file-saver";
import { ZoneGroupService } from "src/app/shared/generated/api/zone-group.service";
import { IconComponent } from "../icon/icon.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { ConfirmOptions } from "src/app/shared/services/confirm/confirm-options";
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";
import { PageHeaderComponent } from "../page-header/page-header.component";

@Component({
    selector: "manage-zone-group-card",
    standalone: true,
    imports: [CommonModule, PageHeaderComponent, IconComponent],
    templateUrl: "./manage-zone-group-card.component.html",
    styleUrls: ["./manage-zone-group-card.component.scss"],
})
export class ManageZoneGroupCardComponent {
    public isLoadingSubmit: boolean = false;
    @Input() zoneGroup: ZoneGroupMinimalDto;

    constructor(
        private zoneGroupService: ZoneGroupService,
        private alertService: AlertService,
        private confirmService: ConfirmService
    ) {}

    downloadZoneGroupDataAsCsv() {
        this.isLoadingSubmit = true;
        this.zoneGroupService.geographiesGeographyIDZoneGroupsZoneGroupSlugGet(this.zoneGroup.GeographyID, this.zoneGroup.ZoneGroupSlug).subscribe((response) => {
            saveAs(response, `ZoneGroupDataFor${this.zoneGroup.ZoneGroupName}`);
            this.isLoadingSubmit = false;
        });
    }

    public clearZoneGroupData() {
        this.isLoadingSubmit = true;

        const confirmOptions = {
            title: "Warning",
            message: `You are about to delete all the data for this ${this.zoneGroup.ZoneGroupName}<br /><br />
      Are you sure you wish to proceed?`,
            buttonTextYes: "Delete",
            buttonClassYes: "btn-danger",
            buttonTextNo: "Cancel",
        } as ConfirmOptions;

        this.confirmService.confirm(confirmOptions).then((confirmed) => {
            if (confirmed) {
                this.zoneGroupService.geographiesGeographyIDZoneGroupZoneGroupIDClearDelete(this.zoneGroup.GeographyID, this.zoneGroup.ZoneGroupID).subscribe((response) => {
                    this.isLoadingSubmit = false;
                    this.alertService.pushAlert(new Alert("Successfully cleared Zone Group Data.", AlertContext.Success));
                });
            }
        });
    }
}
