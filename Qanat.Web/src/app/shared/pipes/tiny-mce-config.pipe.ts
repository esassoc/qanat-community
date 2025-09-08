import { Pipe, PipeTransform } from "@angular/core";
import { EditorComponent } from "@tinymce/tinymce-angular";
import TinyMCEHelpers from "../helpers/tiny-mce-helpers";

@Pipe({
    name: "tinyMceConfig",
    standalone: true,
})
/**
 * Pipe to transform the TinyMCE editor configuration.
 * Used for the TinyMCE editor component to prevent the editor from being re-initialized on every change detection cycle.
 */
export class TinyMceConfigPipe implements PipeTransform {
    transform(editor: EditorComponent): unknown {
        return TinyMCEHelpers.DefaultInitConfig(editor);
    }
}
