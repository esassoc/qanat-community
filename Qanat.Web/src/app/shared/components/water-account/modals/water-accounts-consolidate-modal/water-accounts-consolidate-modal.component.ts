import { Component, inject } from "@angular/core";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "water-accounts-consolidate-modal",
    imports: [IconComponent, NoteComponent],
    templateUrl: "./water-accounts-consolidate-modal.component.html",
    styleUrl: "./water-accounts-consolidate-modal.component.scss",
})
export class WaterAccountsConsolidateModalComponent {
    public ref: DialogRef<WaterAccountModificationsContext, boolean> = inject(DialogRef);

    public customRichTextTypeID = CustomRichTextTypeEnum.ConsolidateWaterAccountsDisclaimer;

    constructor() {}

    close() {
        this.ref.close(false);
    }

    confirm() {
        this.ref.close(true);
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
