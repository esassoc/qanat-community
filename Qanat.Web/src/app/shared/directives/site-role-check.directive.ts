import { OnInit, Directive, Input, TemplateRef, ViewContainerRef, OnDestroy } from "@angular/core";
import { RoleEnum } from "../generated/enum/role-enum";
import { UserDto } from "../generated/model/models";

@Directive({
    selector: "[withRole]",
    standalone: true,
})
export class WithRoleDirective implements OnInit, OnDestroy {
    @Input() set withRole(roleCheck: RoleCheck) {
        if (roleCheck.roles.includes(roleCheck.currentUser.Role.RoleID)) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        } else {
            this.viewContainer.clear();
        }
    }

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef
    ) {}

    ngOnInit(): void {}

    ngOnDestroy(): void {}
}

export interface RoleCheck {
    currentUser: UserDto;
    roles: RoleEnum[];
}
