import { AnimationBuilder, AnimationMetadata, animate, style } from "@angular/animations";
import { OnInit, Directive, Input, ElementRef, Renderer2 } from "@angular/core";

@Directive({
    selector: "[loadingSpinner]",
    standalone: true,
})
export class LoadingDirective implements OnInit {
    @Input()
    set loadingSpinner(loadingSpinner: ILoadingSpinnerOptions) {
        const metadata = loadingSpinner.isLoading ? this.fadeIn() : this.fadeOut();
        const factory = this.builder.build(metadata);
        const player = factory.create(this.loadingDiv);
        player.play();

        const containerMetadata = loadingSpinner.isLoading ? this.fadeInContainer(loadingSpinner.loadingHeight) : this.fadeOutContainer(loadingSpinner.loadingHeight);
        const factory2 = this.builder.build(containerMetadata);
        const player2 = factory2.create(this.el.nativeElement);
        player2.play();
    }

    private loadingDiv: HTMLElement;

    constructor(
        private el: ElementRef,
        private builder: AnimationBuilder,
        private renderer: Renderer2
    ) {
        this.loadingDiv = this.renderer.createElement("div");
        this.renderer.addClass(this.el.nativeElement, "has-spinner");

        // this.renderer.setStyle(this.el.nativeElement, 'min-height', `${this.loadingHeight}px`)
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

    private fadeInContainer(loadingHeight: number): AnimationMetadata[] {
        return [style({ minHeight: `*` }), animate("0ms", style({ minHeight: `${loadingHeight ?? 0}px` }))];
    }
    private fadeOutContainer(loadingHeight: number): AnimationMetadata[] {
        return [style({ minHeight: `*` }), animate("100ms ease-in", style({ minHeight: "0" }))];
    }

    private fadeIn(): AnimationMetadata[] {
        return [style({ opacity: 1 }), animate("50ms ease-in", style({ opacity: 1 }))];
    }

    private fadeOut(): AnimationMetadata[] {
        return [style({ opacity: "*" }), animate("100ms ease-in", style({ opacity: 0, pointerEvents: "none" }))];
    }
}

export interface ILoadingSpinnerOptions {
    isLoading: boolean;
    loadingHeight?: number | null;
}
