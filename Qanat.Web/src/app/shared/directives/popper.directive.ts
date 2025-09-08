import { ApplicationRef, ComponentFactoryResolver, ComponentRef, Directive, ElementRef, HostListener, Input, Renderer2, TemplateRef, ViewContainerRef } from "@angular/core";
import { createPopper, Instance } from "@popperjs/core";
import { AppComponent } from "src/app/app.component";
import { PopperComponent } from "../components/popper/popper.component";
import { PopperService } from "../services/popper.service";

@Directive({
    selector: "[popper]",
    standalone: true,
})
export class PopperDirective {
    @Input() popper: TemplateRef<any>;
    @Input() popperTitle: TemplateRef<any> | string;
    @Input() showOnHover: boolean = false;
    @Input() useBodyContainer: boolean = false;
    @Input() popperOptions?: any = {
        placement: "top",
        modifiers: [
            {
                name: "offset",
                options: {
                    offset: ({ placement, reference, popper }) => {
                        return [0, 5];
                    },
                },
            },
        ],
    };
    @HostListener("click", ["$event"]) onClick(event) {
        if (this.showOnHover) return;
        event.stopPropagation();
        this.popperShown = !this.popperShown;
        this.toggleDisplay();
    }

    @HostListener("mouseenter", ["$event"]) onHoverOver(event) {
        if (!this.showOnHover) return;
        event.stopPropagation();
        this.popperShown = true;
        this.toggleDisplay();
    }

    @HostListener("mouseleave", ["$event"]) onHoverOut(event) {
        if (!this.showOnHover) return;
        event.stopPropagation();
        this.popperShown = false;
        this.toggleDisplay();
    }

    private popperInstance: Instance;
    public popperShown: boolean = false;
    private popperComponentRef: ComponentRef<PopperComponent>;

    constructor(
        private readonly el: ElementRef,
        private componentFactoryResolver: ComponentFactoryResolver,
        private renderer: Renderer2,
        private popperService: PopperService,
        private popperViewRef: ViewContainerRef,
        private applicationRef: ApplicationRef
    ) {
        this.listenForClicksOut();
    }

    listenForClicksOut(): void {
        this.renderer.listen("window", "click", (e: Event) => {
            if (
                e.target !== this.el.nativeElement &&
                !this.el.nativeElement.contains(e.target) &&
                e.target !== this.popperComponentRef.location.nativeElement &&
                !this.popperComponentRef.location.nativeElement.contains(e.target)
            ) {
                this.popperShown = false;
                this.toggleDisplay();
            } else {
                // update the popper instance since click events elsewhere in the popper might have changed the layout
                this.popperInstance.update();
            }
        });
    }

    ngOnInit(): void {
        const viewRef = this.useBodyContainer ? (this.applicationRef.components[0].instance as AppComponent).viewRef : this.popperViewRef;

        this.popperComponentRef = this.createPopperComponentAtViewRef(viewRef);
        this.popperInstance = createPopper(this.el.nativeElement, this.popperComponentRef.location.nativeElement, this.popperOptions);
        this.popperService.pushPopperInstance(this.popperInstance);
        this.toggleDisplay();
    }

    createPopperComponentAtViewRef(viewRef: ViewContainerRef): ComponentRef<PopperComponent> {
        const component = viewRef.createComponent<PopperComponent>(this.componentFactoryResolver.resolveComponentFactory(PopperComponent));
        component.instance.context = this.popper;
        component.instance.title = this.popperTitle;
        component.location.nativeElement.setAttribute("data-popper", "");
        component.changeDetectorRef.detectChanges();
        return component;
    }

    ngOnDestroy(): void {
        this.popperService.removePopperInstance(this.popperInstance);
        this.popperInstance.destroy();
        this.popperComponentRef.destroy();
    }

    toggleDisplay(): void {
        if (this.popperShown) {
            this.popperComponentRef.location.nativeElement.setAttribute("data-show", "");
            this.popperService.clearOtherPoppers(this.popperInstance);
        } else {
            this.popperComponentRef.location.nativeElement.removeAttribute("data-show");
        }
        this.popperInstance.update();
    }
}
