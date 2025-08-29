import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { slideAnimation } from "src/app/shared/animations/slide.animation";
import { IsActiveMatchOptions, Params, RouterLink, RouterLinkActive } from "@angular/router";
import { GeographyFlagCheck, WithGeographyFlagDirective } from "src/app/shared/directives/with-geography-flag.directive";
import { IconComponent, IconInterface } from "../icon/icon.component";

@Component({
    selector: "dashboard-menu",
    imports: [CommonModule, IconComponent, RouterLink, RouterLinkActive, WithGeographyFlagDirective],
    templateUrl: "./dashboard-menu.component.html",
    styleUrls: ["./dashboard-menu.component.scss"],
    animations: [slideAnimation]
})
export class DashboardMenuComponent {
    @Input() dashboardMenu: DashboardMenu;
    @Input() viewingDetailPage: boolean; // this is to prevent the active class from being put on the parent for the nav

    public defaultSubItemRouterLinkActiveOptions: IsActiveMatchOptions = {
        matrixParams: "ignored",
        queryParams: "ignored",
        fragment: "exact",
        paths: "exact",
    };

    toggleDropdown(item: DashboardMenuItem) {
        item.isExpanded = !item.isExpanded;
    }
}

export interface IDashboardMenuItem {
    routerLink?: string | string[];
    title: string;
    icon?: IconInterface; // Specify the type of IconInterface
    isDropdown?: boolean;
}

export class DashboardMenu {
    menuItems: DashboardMenuItem[];
}

export class DashboardMenuItem implements IDashboardMenuItem {
    routerLink?: string | string[];
    fragment?: string;
    title: string;
    icon?: IconInterface;
    isDropdown?: boolean;
    preventCollapse?: boolean;
    menuItems?: DashboardMenuItem[];
    isExpanded?: boolean = false;
    isDisabled?: boolean = false;
    cssClasses?: string;
    routerLinkActiveOptions?: IsActiveMatchOptions;
    queryParams: Params;
    withGeographyFlag: GeographyFlagCheck;
    hidden?: boolean = false; //MK 10/10/2024 -- This is used so that we can have a menu that is styled like a submenu but is not actually a submenu (see WaterDetailLayoutComponent for an example).
}
