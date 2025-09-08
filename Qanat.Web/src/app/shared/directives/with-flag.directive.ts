import { Directive, Input, TemplateRef, ViewContainerRef } from "@angular/core";
import { UserDto } from "../generated/model/models";
import { FlagEnum } from "../generated/enum/flag-enum";

@Directive({
    selector: "[withFlag]",
    standalone: true,
})
export class WithFlagDirective {
    @Input() set withFlag(flagCheck: FlagCheck) {
        this.viewContainer.clear(); // always clear up front
        try {
            const enumKey = FlagEnum[flagCheck.flag];
            const userHasFlag = flagCheck.currentUser.Flags[enumKey];
            if (userHasFlag) {
                this.viewContainer.createEmbeddedView(this.templateRef);
            } else {
                this.viewContainer.clear();
            }
        } catch (error) {
            this.viewContainer.clear();
        }
    }

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef
    ) {}
}

export interface FlagCheck {
    currentUser: UserDto;
    flag: FlagEnum;
}
