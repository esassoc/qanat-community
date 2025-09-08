import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "map",
    standalone: true,
})
export class MapPipe implements PipeTransform {
    transform(input: any[], key: string): any {
        if (!input) return;
        return input.map((value) => value[key]);
    }
}
