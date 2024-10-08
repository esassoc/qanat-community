import { Directive, Input, ElementRef, HostListener, HostBinding, Renderer2, OnDestroy } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { Subscription } from "rxjs";

@Directive({
    selector: "[dropdownToggle]",
    exportAs: "dropdownToggleName",
    standalone: true,
})
export class DropdownToggleDirective implements OnDestroy {
    private routerNavigationEndSubscription = Subscription.EMPTY;
    private classString: string = "active";
    @Input() dropdownToggle: any;

    @HostBinding("class.active") showMenu: boolean = false;

    @HostListener("click", ["$event"]) onClick(event) {
        this.showMenu = !this.showMenu;
        this.toggleMenu();
    }

    // clicking outside of the element(s) that have the dropdownToggle directive will close the dropdown
    @HostListener("window:click", ["$event"]) onWindowClick(event) {
        if (
            event.target !== this.el.nativeElement &&
            !this.el.nativeElement.contains(event.target) &&
            event.target !== this.dropdownToggle &&
            !this.dropdownToggle.contains(event.target)
        ) {
            this.showMenu = false;
            this.toggleMenu();
        }
    }

    @HostListener("document:keydown.escape", ["$event"]) onKeydownHandler(event: KeyboardEvent) {
        this.showMenu = false;
        this.toggleMenu();
    }

    constructor(
        private el: ElementRef,
        private renderer: Renderer2,
        private router: Router
    ) {
        this.routerNavigationEndSubscription = router.events.subscribe((e) => {
            if (e instanceof NavigationEnd) {
                this.showMenu = false;
                this.toggleMenu();
            }
        });
    }

    ngOnDestroy(): void {
        this.routerNavigationEndSubscription.unsubscribe();
    }

    toggleMenu(show: boolean = null) {
        if (show != null) {
            this.showMenu = show;
        }
        if (this.showMenu) {
            this.renderer.addClass(this.dropdownToggle, this.classString);
        } else {
            this.renderer.removeClass(this.dropdownToggle, this.classString);
        }
    }
}
