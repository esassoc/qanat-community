import { Directive, Input, TemplateRef, ViewContainerRef } from "@angular/core";
import { FlagEnum } from "../generated/enum/flag-enum";
import { FlagCheck } from "./with-flag.directive";

@Directive({
    selector: "[withGeographyFlag]",
    standalone: true,
})
export class WithGeographyFlagDirective {
    @Input() set withGeographyFlag(flagCheck: GeographyFlagCheck) {
        this.viewContainer.clear(); // always clear up front
        try {
            const enumKey = FlagEnum[flagCheck.flag];
            const userHasFlag = flagCheck.currentUser?.Flags[enumKey] ?? false;
            const userHasGeographyFlag = flagCheck.currentUser?.GeographyFlags[flagCheck.geographyID]
                ? flagCheck.currentUser?.GeographyFlags[flagCheck.geographyID][enumKey]
                : false;

            const userHasGeographyFlagForAnyGeography = flagCheck.currentUser
                ? Object.values(flagCheck.currentUser?.GeographyFlags)
                      .map((x) => x[enumKey])
                      .some((x) => x)
                : false;

            if (userHasFlag || userHasGeographyFlag || (flagCheck.forAnyGeography && userHasGeographyFlagForAnyGeography)) {
                this.viewContainer.createEmbeddedView(this.templateRef);
            } else {
                this.viewContainer.clear();
            }
        } catch (error) {
            console.error(error);
            this.viewContainer.clear();
        }
    }

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef
    ) {}
}

export interface GeographyFlagCheck extends FlagCheck {
    geographyID?: number;
    forAnyGeography?: boolean;
}
