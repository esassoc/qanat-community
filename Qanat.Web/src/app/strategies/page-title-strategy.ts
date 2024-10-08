import { Injectable } from "@angular/core";
import { Title } from "@angular/platform-browser";
import { TitleStrategy, RouterStateSnapshot } from "@angular/router";

@Injectable({ providedIn: "root" })
export class PageTitleStrategy extends TitleStrategy {
    constructor(private readonly title: Title) {
        super();
    }

    override updateTitle(routerState: RouterStateSnapshot) {
        const titles = this.getTitleHierarchyFromActivatedRouteShapshot(routerState);
        if (titles.length > 0) {
            const fullTitle = titles.join(" - ");
            this.title.setTitle(`${fullTitle} | Groundwater Accounting Platform`);
        } else {
            this.title.setTitle(`Groundwater Accounting Platform`);
        }
    }

    private getTitleHierarchyFromActivatedRouteShapshot(routerStateSnapshot: RouterStateSnapshot): string[] {
        let activatedRouteSnapshot = routerStateSnapshot.root;
        let titles = [];
        let params = [];
        while (activatedRouteSnapshot.firstChild != null) {
            activatedRouteSnapshot = activatedRouteSnapshot.firstChild;
            titles = activatedRouteSnapshot.title ? [...titles, activatedRouteSnapshot.title] : [...titles];
            // lets just take the last params
            params = Object.values(activatedRouteSnapshot.params);
        }
        titles = [...titles, ...params];
        return titles;
    }
}
