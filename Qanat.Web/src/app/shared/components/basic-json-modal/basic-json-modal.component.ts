import { JsonPipe } from "@angular/common";
import { Component, inject, OnInit } from "@angular/core";
import { DialogRef } from "@ngneat/dialog";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { NoteComponent } from "src/app/shared/components/note/note.component";

@Component({
    selector: "basic-json-modal",
    templateUrl: "./basic-json-modal.component.html",
    styleUrl: "./basic-json-modal.component.scss",
    imports: [JsonPipe, NoteComponent, IconComponent],
})
export class BasicJsonModalComponent implements OnInit {
    public ref: DialogRef<BasicJsonModalContext, void> = inject(DialogRef);
    public json: JSON;

    constructor() {}

    ngOnInit(): void {
        switch (typeof this.ref.data.json) {
            case "string":
                this.json = JSON.parse(this.ref.data.json);
                break;
            case "object":
                this.json = this.ref.data.json;
                break;
            default:
                console.error("The json input provided must be either a string or a JSON object.");
        }
    }

    close(): void {
        this.ref.close();
    }
}

export class BasicJsonModalContext {
    title: string;
    json: string | JSON;
    explanationHtml: string;
}
