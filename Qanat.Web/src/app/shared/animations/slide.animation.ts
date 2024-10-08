import { trigger, transition, style, animate, state } from "@angular/animations";

export const slideAnimation = trigger("slideState", [
    state("collapsed", style({ height: "0px" })),
    state("expanded", style({ height: "*" })),
    transition("collapsed => expanded", animate("100ms ease-in-out")),
    transition("expanded => collapsed", animate("100ms ease-in-out")),
]);
