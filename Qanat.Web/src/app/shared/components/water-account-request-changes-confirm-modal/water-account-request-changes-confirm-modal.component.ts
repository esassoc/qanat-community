import { Component, ComponentRef } from "@angular/core";
import { ModalService } from "src/app/shared/services/modal/modal.service";
import { ModalComponent } from "src/app/shared/components/modal/modal.component";
import { WaterAccountModificationsContext } from "../water-accounts-consolidate-modal/water-accounts-consolidate-modal.component";
import { CustomRichTextTypeEnum } from "src/app/shared/generated/enum/custom-rich-text-type-enum";
import { NoteComponent } from "src/app/shared/components/note/note.component";
import { CustomRichTextComponent } from "src/app/shared/components/custom-rich-text/custom-rich-text.component";
import { FormsModule } from "@angular/forms";
import { IconComponent } from "../icon/icon.component";

@Component({
    selector: "water-account-request-changes-confirm-modal",
    standalone: true,
    imports: [IconComponent, NoteComponent, CustomRichTextComponent, FormsModule],
    templateUrl: "./water-account-request-changes-confirm-modal.component.html",
    styleUrl: "./water-account-request-changes-confirm-modal.component.scss",
})
export class WaterAccountRequestChangesConfirmModalComponent {
    private modalComponentRef: ComponentRef<ModalComponent>;
    public modalContext: WaterAccountModificationsContext;

    public customRichTextTypeID = CustomRichTextTypeEnum.WaterAccountRequestChangesCertification;
    public certificationAccepted: boolean = false;

    constructor(private modalService: ModalService) {}

    close() {
        this.modalService.close(this.modalComponentRef, false);
    }

    confirm() {
        this.modalService.close(this.modalComponentRef, true);
    }
}
