import { AnimationBuilder } from "@angular/animations";
import { OnInit, Directive, Input, ElementRef, Renderer2 } from "@angular/core";

@Directive({
    selector: "[buttonLoading]",
    standalone: true,
})
export class ButtonLoadingDirective implements OnInit {
    @Input()
    set buttonLoading(isLoading: boolean) {
        if (isLoading) {
            // this.renderer.appendChild(this.el.nativeElement, this.loadingDiv);
            this.renderer.insertBefore(this.el.nativeElement, this.loadingDiv, this.el.nativeElement.firstChild);
        } else {
            this.renderer.removeChild(this.el.nativeElement, this.loadingDiv);
        }
    }

    private loadingDiv: HTMLElement;

    constructor(
        private el: ElementRef,
        private builder: AnimationBuilder,
        private renderer: Renderer2
    ) {
        this.loadingDiv = this.renderer.createElement("i");
        this.renderer.addClass(this.loadingDiv, "fas");
        this.renderer.addClass(this.loadingDiv, "fa-spinner");
        this.renderer.addClass(this.loadingDiv, "fa-spin");
    }

    ngOnInit(): void {}
}
