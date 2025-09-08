import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "zipCode",
    standalone: true,
})
export class ZipCodePipe implements PipeTransform {
    transform(value: string): string {
        if (!value) {
            return value;
        }
        if (value.toString().length === 9) {
            return value.toString().slice(0, 5) + "-" + value.toString().slice(5);
        } else if (value.toString().length === 5) {
            return value.toString();
        } else {
            return value;
        }
    }

    gridFilterTextFormatter(from: string) {
        return from.replace("-", "").replace(" ", "");
    }
}
