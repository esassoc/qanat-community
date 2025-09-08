import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "commaJoin",
    standalone: true,
})
export class CommaJoinPipe implements PipeTransform {
    transform(input: any[], key: string): any {
        return input.map((value) => value[key]).join(", ");
    }
}
