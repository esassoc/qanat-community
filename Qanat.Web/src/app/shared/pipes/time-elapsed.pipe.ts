import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: "timeElapsed",
    standalone: true,
})

// displays how much time has elapsed since the passed Date or date string, as follows:
//      0 - 45 seconds              less than a minute
//      45 - 90 seconds             a minute
//      90 seconds - 45 minutes     [n] minutes"
//      45 - 90 minutes             an hour
//      90 minutes - 22 hours       [n] hours
//      22 - 36 hours               a day
//      36 hours - 25 days          [n] days
//      25 - 45 days                a month
//      45 - 345 days               [n] months
//      345 - 545 days (1.5 years)  a year
//      546 days+                   [n] years
export class TimeElapsedPipe implements PipeTransform {
    transform(input: Date | string, labelOverride?: string): any {
        var pastDate = new Date(input);
        var currentDate = new Date();

        var seconds = (currentDate.getTime() - pastDate.getTime()) / 1000;
        var minutes = Math.round(Math.abs(seconds / 60));
        var hours = Math.round(Math.abs(minutes / 60));
        var days = Math.round(Math.abs(hours / 24));
        var months = Math.round(Math.abs(days / 30.416));
        var years = Math.round(Math.abs(days / 365));

        if (seconds <= 45) {
            return "less than a minute";
        } else if (minutes <= 45) {
            return `${minutes} minute${minutes == 1 ? "" : "s"}`;
        } else if (hours <= 22) {
            return `${hours} hour${hours == 1 ? "" : "s"}`;
        } else if (days <= 25) {
            return `${days} day${days == 1 ? "" : "s"}`;
        } else if (days <= 345) {
            return `${months} month${days == 1 ? "" : "s"}`;
        } else {
            return `${years} year${years == 1 ? "" : "s"}`;
        }
    }
}
