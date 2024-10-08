import { AlertContext } from "./enums/alert-context.enum";

export class Alert {
    constructor(
        public message: string,
        public context: AlertContext = AlertContext.Primary,
        public dismissible: boolean = true,
        public uniqueCode: string = ""
    ) {}
}
