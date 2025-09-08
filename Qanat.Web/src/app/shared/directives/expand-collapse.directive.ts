import { Directive, HostBinding, HostListener, Input, Renderer2 } from "@angular/core";

@Directive({
    selector: "[expandCollapse]",
    standalone: true,
})
export class ExpandCollapseDirective {
    @Input() expandCollapse: HTMLElement;
    @Input() startOpen: boolean = false;
    @HostBinding("class.open") isOpen: boolean = false;
    @HostListener("click", ["$event"]) onClick(event) {
        this.isOpen = !this.isOpen;
        this.toggleExpand();
    }

    public classString: string = "open";

    constructor(private renderer: Renderer2) {}

    ngOnInit() {
        if (this.startOpen) this.isOpen = true;
        this.toggleExpand();
    }

    private toggleExpand(): void {
        if (this.isOpen) {
            this.renderer.addClass(this.expandCollapse, this.classString);
        } else {
            this.renderer.removeClass(this.expandCollapse, this.classString);
        }
    }
}
