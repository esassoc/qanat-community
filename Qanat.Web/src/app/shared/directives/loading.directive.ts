import { AnimationBuilder, AnimationMetadata, animate, style } from "@angular/animations";
import { OnInit, Directive, Input, ElementRef, Renderer2, AfterViewInit } from "@angular/core";

@Directive({
    selector: "[loadingSpinner]",
    standalone: true,
})
export class LoadingDirective implements OnInit, AfterViewInit {
    private spinnerOptions: ILoadingSpinnerOptions;

    @Input()
    set loadingSpinner(loadingSpinner: ILoadingSpinnerOptions) {
        const metadata = loadingSpinner.isLoading ? this.fadeIn(loadingSpinner.opacity) : this.fadeOut();
        const factory = this.builder.build(metadata);
        const player = factory.create(this.loadingDiv);
        player.play();

        const containerMetadata = loadingSpinner.isLoading ? this.fadeInContainer(loadingSpinner.loadingHeight) : this.fadeOutContainer(loadingSpinner.loadingHeight);
        const factory2 = this.builder.build(containerMetadata);
        const player2 = factory2.create(this.el.nativeElement);
        player2.play();
        this.spinnerOptions = loadingSpinner;
    }

    private loadingDiv: HTMLElement;

    constructor(
        private el: ElementRef,
        private builder: AnimationBuilder,
        private renderer: Renderer2
    ) {
        this.loadingDiv = this.renderer.createElement("div");
        this.renderer.addClass(this.el.nativeElement, "has-spinner");

        this.renderer.addClass(this.loadingDiv, "spinner-container");

        const circle = this.renderer.createElement("div");
        this.renderer.addClass(circle, "circle");

        const wave = this.renderer.createElement("div");
        this.renderer.addClass(wave, "wave");

        this.renderer.appendChild(circle, wave);
        this.renderer.appendChild(this.loadingDiv, circle);
        this.renderer.appendChild(this.el.nativeElement, this.loadingDiv);
    }

    ngOnInit(): void {}

    ngAfterViewInit(): void {
        if (this.spinnerOptions) {
            if (this.spinnerOptions.opacity) {
                this.renderer.setStyle(this.loadingDiv, "background-color", `rgba(0, 0, 0, ${this.spinnerOptions.opacity})`);
                this.renderer.setStyle(this.el.nativeElement, "height", "100%");
            }
        }
    }

    private fadeInContainer(loadingHeight: number): AnimationMetadata[] {
        return [style({ minHeight: `*` }), animate("0ms", style({ minHeight: `${loadingHeight ?? 0}px` }))];
    }

    private fadeOutContainer(loadingHeight: number): AnimationMetadata[] {
        return [style({ minHeight: `*` }), animate("100ms ease-in", style({ minHeight: "0" }))];
    }

    private fadeIn(loadingOpacity: number = 1): AnimationMetadata[] {
        if (this.spinnerOptions?.opacity) {
            this.renderer.setStyle(this.el.nativeElement, "height", "100%");
        }

        return [style({ opacity: loadingOpacity }), animate("50ms ease-in", style({ opacity: 1 }))];
    }

    private fadeOut(): AnimationMetadata[] {
        if (this.spinnerOptions?.opacity) {
            this.renderer.setStyle(this.el.nativeElement, "height", "0px");
        }

        return [style({ opacity: "*" }), animate("100ms ease-in", style({ opacity: 0, pointerEvents: "none" }))];
    }
}

export interface ILoadingSpinnerOptions {
    isLoading: boolean;
    loadingHeight?: number | null;
    opacity?: number;
}
