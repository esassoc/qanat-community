import { Component, Input } from "@angular/core";

@Component({
    selector: "note",
    templateUrl: "./note.component.html",
    styleUrls: ["./note.component.scss"],
    standalone: true,
})
export class NoteComponent {
    @Input() noteType: typeof NoteType = "default";
}

export let NoteType: "default" | "danger" | "info" | "success" | "warning";
