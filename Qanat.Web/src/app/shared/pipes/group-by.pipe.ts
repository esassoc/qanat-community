import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "groupBy",
    standalone: true,
})
export class GroupByPipe implements PipeTransform {
    transform<T>(array: any[], property: string): ReadonlyMap<string, T[]> {
        return array.reduce((result, obj) => {
            const keys = property.split(".");
            const key = keys.reduce((acc, key) => acc[key], obj);

            if (!result[key]) {
                result[key] = [];
            }
            result[key].push(obj);
            return result;
        }, {});
    }
}
