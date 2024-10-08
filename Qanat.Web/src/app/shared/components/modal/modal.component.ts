import { Component, ComponentRef, ElementRef, HostBinding, HostListener, Input, TemplateRef, ViewChild, ViewContainerRef } from "@angular/core";
import { ModalOptions, ModalService, ModalSizeEnum, ModalThemeEnum } from "../../services/modal/modal.service";
import { NgTemplateOutlet } from "@angular/common";
@Component({
    selector: "modal",
    templateUrl: "./modal.component.html",
    styleUrls: ["./modal.component.scss"],
    standalone: true,
    imports: [NgTemplateOutlet],
})
export class ModalComponent {
    @ViewChild("dialog") dialog: ElementRef;
    @ViewChild("vc", { read: ViewContainerRef }) vc: ViewContainerRef;
    @Input() context: TemplateRef<any>;

    private _modalOptions: ModalOptions = null;
    @Input() set modalOptions(value: ModalOptions) {
        if (value?.ModalSize != null) {
            this.modalSize = value.ModalSize;
        }
        if (value?.ModalTheme != null) {
            this.modalTheme = value.ModalTheme;
        }
        if (value?.TopLayer != null) {
            this.topLayer = value.TopLayer;
        }

        if (value?.OverflowVisible != null) {
            this.overflowVisible = value.OverflowVisible;
        }
        if (value?.CloseOnClickOut != null) {
            this.closeOnClickOut = value.CloseOnClickOut;
        }
    }
    get modalOptions(): ModalOptions {
        return this._modalOptions;
    }

    @HostBinding("class.not-top-layer") get notTopLayer() {
        return !this.topLayer;
    }

    public modalSize: ModalSizeEnum = ModalSizeEnum.Medium;
    public ModalSizeEnum = ModalSizeEnum;

    public modalTheme: ModalThemeEnum = ModalThemeEnum.Primary;
    public ModalThemeEnum = ModalThemeEnum;

    public topLayer: boolean = true;
    public closeOnClickOut: boolean = true;
    public overflowVisible: boolean = false;

    public modalComponentRef: ComponentRef<ModalComponent>;
    /**
     * The promise that is resolved when the modal is closed and rejected when the modal is dismissed.
     */
    result: Promise<any>;
    private _resolve: (result?: any) => void;
    private _reject: (reason?: any) => void;

    constructor(
        public viewRef: ViewContainerRef,
        private modalService: ModalService
    ) {
        this.result = new Promise((resolve, reject) => {
            this._resolve = resolve;
            this._reject = reject;
        });
        this.result.then(null, () => {});
    }

    // This host listener is here to close the modal on click outside ONLY IF it's not a top-layer modal
    private alreadyClicked = false;
    @HostListener("document:click", ["$event"])
    public onClick(event) {
        if (!this.alreadyClicked) {
            this.alreadyClicked = true;
            return;
        }
        if (!this.closeOnClickOut) return;
        const eventTarget = event.target;
        const clickedOutside = this.dialog.nativeElement != eventTarget && !this.dialog.nativeElement.contains(eventTarget);
        if (clickedOutside && this.topLayer === false) {
            this.modalService.close(this.modalComponentRef);
        }
    }

    close(result: boolean = true): void {
        (this.dialog.nativeElement as HTMLDialogElement).close();
        this._resolve(result);
    }
}
