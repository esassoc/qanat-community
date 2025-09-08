import { Injectable } from "@angular/core";
import { Instance } from "@popperjs/core";

@Injectable({
    providedIn: "root",
})
export class PopperService {
    private poppers: Instance[] = [];

    constructor() {}

    pushPopperInstance(popper: Instance) {
        this.poppers.push(popper);
    }

    removePopperInstance(popper: Instance) {
        const indexOfPopper = this.poppers.findIndex((x) => x == popper);
        this.poppers.splice(indexOfPopper, 1);
    }

    clearOtherPoppers(popper: Instance) {
        const otherPoppers = this.poppers.filter((x) => x != popper);
        otherPoppers.forEach((x) => {
            x.state.elements.popper.removeAttribute("data-show");
            x.update();
        });
    }
}
