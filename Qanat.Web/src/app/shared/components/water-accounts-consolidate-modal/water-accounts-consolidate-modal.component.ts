import { Component, ComponentRef } from "@angular/core";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "water-accounts-consolidate-modal",
    standalone: true,
    imports: [IconComponent, NoteComponent],
    templateUrl: "./water-accounts-consolidate-modal.component.html",
    styleUrl: "./water-accounts-consolidate-modal.component.scss",
})
export class WaterAccountsConsolidateModalComponent {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: WaterAccountModificationsContext;

    public customRichTextTypeID = CustomRichTextTypeEnum.ConsolidateWaterAccountsDisclaimer;

    constructor(private modalService: ModalService) {}

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    confirm() {
        this.modalService.close(this.modalComponentRef, true);
    }
}

export class WaterAccountModificationsContext {
    GeographyID: number;
    WaterAccounts: ModifiedWaterAccountContext[];
    HasInvalidZones?: boolean;
}

export class ModifiedWaterAccountContext {
    WaterAccountID: number;
    WaterAccountName: string;
    WaterAccountNumber: number;

    ParcelsCount: number;
    AddedParcelsCount: number;
    RemovedParcelsCount: number;

    ExistingParcelsCount: number;
    ModifiedParcelsCount: number;
}
