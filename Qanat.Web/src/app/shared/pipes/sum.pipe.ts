import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "sum",
    standalone: true,
})
export class SumPipe implements PipeTransform {
    transform(input: any[], key: string): any {
        if (!input || input.length < 1) return 0;
        return input.map((value) => value[key]).reduce((a, b) => a + b);
    }
}
