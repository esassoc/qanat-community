import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "phone",
    standalone: true,
})
export class PhonePipe implements PipeTransform {
    transform(value: string): string {
        // Basic regular expression to match common US phone number patterns
        const phoneRegex = /^(\d{3})(\d{3})(\d{4})$/;

        if (phoneRegex.test(value)) {
            const matches = value.match(phoneRegex);
            return `(${matches[1]}) ${matches[2]}-${matches[3]}`;
        } else {
            return value; // Return original value if it doesn't match
        }
    }

    gridFilterTextFormatter(from: string) {
        return from.replace("(", "").replace(")", "").replace("-", "").replace(" ", "");
    }
}
