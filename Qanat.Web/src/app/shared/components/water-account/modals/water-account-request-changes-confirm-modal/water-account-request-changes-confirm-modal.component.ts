import { Component, inject } from "@angular/core";
import { WaterAccountModificationsContext } from "../water-accounts-consolidate-modal/water-accounts-consolidate-modal.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FormsModule } from "@angular/forms";
import { IconComponent } from "../../../icon/icon.component";
import { DialogRef } from "@ngneat/dialog";

@Component({
    selector: "water-account-request-changes-confirm-modal",
    imports: [IconComponent, NoteComponent, CustomRichTextComponent, FormsModule],
    templateUrl: "./water-account-request-changes-confirm-modal.component.html",
    styleUrl: "./water-account-request-changes-confirm-modal.component.scss",
})
export class WaterAccountRequestChangesConfirmModalComponent {
    public ref: DialogRef<WaterAccountModificationsContext, boolean> = inject(DialogRef);

    public customRichTextTypeID = CustomRichTextTypeEnum.WaterAccountRequestChangesCertification;
    public certificationAccepted: boolean = false;

    constructor() {}

    close() {
        this.ref.close(false);
    }

    confirm() {
        this.ref.close(true);
    }
}
