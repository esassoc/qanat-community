import { ApplicationRef, ComponentRef, Injectable, TemplateRef, Type, ViewContainerRef } from "@angular/core";
import { AppComponent } from "src/app/app.component";
import { ModalComponent } from "../../components/modal/modal.component";
import { Subject } from "rxjs";
import { AlertService } from "../alert.service";

export class ModalTemplateComponentRef {
    templateRef: TemplateRef<any>;
    componentRef: ComponentRef<ModalComponent>;
    viewContainerRef: ViewContainerRef;
    constructor(templateRef: TemplateRef<any>, componentRef: ComponentRef<ModalComponent>, viewContainerRef: ViewContainerRef) {
        this.templateRef = templateRef;
        this.componentRef = componentRef;
        this.viewContainerRef = viewContainerRef;
    }
}

@Injectable({
    providedIn: "root",
})
export class ModalService {
    private bodyViewContainerRef: ViewContainerRef;
    private modals: ModalTemplateComponentRef[] = [];

    private _modalEventSubject = new Subject<ModalEvent>();
    public modalEventObservable = this._modalEventSubject.asObservable();

    constructor(
        private applicationRef: ApplicationRef,
        private alertService: AlertService
    ) {
        this.bodyViewContainerRef = this.applicationRef.components.length > 0 ? (this.applicationRef.components[0].instance as AppComponent).viewRef : null;
    }

    open(content: any, viewContainerRef: ViewContainerRef = null, options: ModalOptions = null, modalContext: any = null): ComponentRef<ModalComponent> {
        // clear any alerts before opening modal
        this.alertService.clearAlerts();

        if (!this.modals.map((x) => x.templateRef).includes(content)) {
            const viewRef = viewContainerRef ?? this.bodyViewContainerRef;

            let modal;
            if (content instanceof TemplateRef) {
                modal = this.createModalAtViewRef(content, viewRef, options);
            } else {
                modal = this.createFromComponent(content, viewRef, options, modalContext);
            }

            this.modals.push(new ModalTemplateComponentRef(content, modal, viewRef));
        }

        const modal = this.modals.find((x) => x.templateRef == content).componentRef;

        if (options?.TopLayer === false) {
            (modal.instance.dialog.nativeElement as HTMLDialogElement).show();
        } else {
            (modal.instance.dialog.nativeElement as HTMLDialogElement).showModal();
        }

        this.addCloseEventListenersToModal(modal);
        return modal;
    }

    createFromComponent(componentType: Type<IModal>, viewRef: ViewContainerRef, options: ModalOptions, modalContext: any = null): ComponentRef<ModalComponent> {
        // create the base modal component
        const modalComponent = viewRef.createComponent(ModalComponent);
        modalComponent.changeDetectorRef.detectChanges();
        modalComponent.instance.modalOptions = options;
        modalComponent.instance.modalComponentRef = modalComponent;

        // inject the componentType specified into the base modal component at the component's view container
        const component = modalComponent.instance.vc.createComponent(componentType);

        // attach the parent modal component to the instantiated modal so we can access it and close it from the component itself
        component.instance.modalComponentRef = modalComponent;
        // attach any context that needs to be provided to the modal
        component.instance.modalContext = modalContext;
        component.changeDetectorRef.detectChanges();

        return modalComponent;
    }

    createModalAtViewRef(content: any, viewRef: ViewContainerRef, options: ModalOptions): ComponentRef<ModalComponent> {
        const component = viewRef.createComponent(ModalComponent);
        component.instance.context = content;
        component.instance.modalOptions = options;
        component.changeDetectorRef.detectChanges();
        return component;
    }

    close(modalComponentRef: ComponentRef<ModalComponent>, result: any = null, emitEvent: ModalEvent = null) {
        const modalRefIndex = this.modals.findIndex((x) => x.componentRef == modalComponentRef);
        if (modalRefIndex >= 0) {
            this.modals[modalRefIndex].componentRef.instance.close(result);
            const modalViewContainerRef = this.modals[modalRefIndex].viewContainerRef;
            const viewIndex = modalViewContainerRef.indexOf(modalComponentRef.hostView);
            modalViewContainerRef.remove(viewIndex);
            this.modals.splice(modalRefIndex, 1);

            if (emitEvent != null) {
                this._modalEventSubject.next(emitEvent);
            } else {
                this._modalEventSubject.next(new ModalCloseEvent(result));
            }
        }
    }

    addCloseEventListenersToModal(modal: ComponentRef<ModalComponent>) {
        // we have to listen for escape key presses on dialogs that are opened with ".show()"
        if (modal?.instance.topLayer === false) {
            (modal.location.nativeElement as HTMLDialogElement).addEventListener("keyup", (event) => {
                if (event.key === "Escape") {
                    event.preventDefault();
                    this.close(modal, false);
                }
            });
        }
        // for dialogs that are opened with ".showModal()" we can listen to the modal cancel event
        else {
            (modal.location.nativeElement as HTMLDialogElement).addEventListener("cancel", (event) => {
                event.preventDefault();
                this.close(modal, false);
            });
        }
    }
}

export interface IModal {
    modalComponentRef: ComponentRef<ModalComponent>;
    modalContext: any;
}

export interface ModalOptions {
    ModalSize: ModalSizeEnum;
    ModalTheme?: ModalThemeEnum;
    TopLayer?: boolean;
    OverflowVisible?: boolean;
    CloseOnClickOut?: boolean;
}

export enum ModalSizeEnum {
    Small = 1,
    Medium = 2,
    Large = 3,
    ExtraLarge = 4,
}

export enum ModalThemeEnum {
    Primary = 1,
    Light = 2,
}

export class ModalEvent {
    public Result: any;
    constructor(result: any) {
        this.Result = result;
    }
}

export class ModalCloseEvent extends ModalEvent {
    constructor(result: any) {
        super(result);
    }
}
