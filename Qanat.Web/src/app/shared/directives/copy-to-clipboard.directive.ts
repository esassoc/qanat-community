import { AfterViewInit, Directive, ElementRef, HostBinding, HostListener, Input, Renderer2 } from "@angular/core";

@Directive({
    selector: "[copyToClipboard]",
    standalone: true,
})
export class CopyToClipboardDirective implements AfterViewInit {
    @Input("copyToClipboard") enabled: boolean = true;
    @Input() copyToClipboardValueOverride: string;
    private button: Element;
    private icon: Element;

    private readonly iconCheckedClasses = "fas fa-check";
    private readonly iconCopyClasses = "far fa-copy";

    @HostListener("mouseover", ["$event"])
    mouseOver(e) {
        if (!this.enabled) return;
        this.renderer.setAttribute(this.button, "style", "opacity:1;");
    }

    @HostListener("mouseout", ["$event"])
    mouseOut(e) {
        if (!this.enabled) return;
        this.renderer.setAttribute(this.button, "style", "opacity:0.25;");
    }

    @HostBinding("style.cursor") get cursor() {
        return this.enabled ? "pointer" : "default";
    }

    @HostListener("click", ["$event"])
    click(e) {
        if (!this.enabled) return;
        this.copyValueToClipboard();
    }

    constructor(
        private el: ElementRef,
        private renderer: Renderer2
    ) {}
    ngAfterViewInit(): void {
        if (this.enabled) {
            this.button = document.createElement("button");
            this.renderer.setAttribute(this.button, "class", "clipboard-copy-button");
            this.renderer.setAttribute(this.button, "title", "Copy to clipboard");
            this.renderer.setAttribute(this.button, "style", "opacity:0.25;");

            this.icon = document.createElement("i");
            this.renderer.setAttribute(this.icon, "class", this.iconCopyClasses);
            this.renderer.appendChild(this.button, this.icon);
            this.button.addEventListener("click", () => this.copyValueToClipboard());

            this.renderer.appendChild(this.el.nativeElement, this.button);
            this.renderer.addClass(this.el.nativeElement, "clipboard-copy-wrapper");
        }
    }

    private copyValueToClipboard() {
        this.renderer.addClass(this.button, "copied");
        this.renderer.setAttribute(this.button, "title", "Copied!");

        this.renderer.setAttribute(this.icon, "class", this.iconCheckedClasses);

        if (this.copyToClipboardValueOverride) {
            navigator.clipboard.writeText(this.copyToClipboardValueOverride);
        } else {
            navigator.clipboard.writeText(this.el.nativeElement.textContent.trim());
        }

        setTimeout(() => {
            this.renderer.removeClass(this.button, "copied");
            this.renderer.setAttribute(this.button, "title", "Copy to clipboard");
            this.renderer.setAttribute(this.icon, "class", this.iconCopyClasses);
        }, 3000);
    }
}
