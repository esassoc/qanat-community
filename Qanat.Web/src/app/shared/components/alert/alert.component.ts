import { Component, Input, OnInit } from "@angular/core";
import { Alert } from "../../models/alert";
import { AlertContext } from "../../models/enums/alert-context.enum";
import { AlertService } from "../../services/alert.service";
import { NgIf } from "@angular/common";

@Component({
    selector: "alert",
    templateUrl: "./alert.component.html",
    styleUrls: ["./alert.component.scss"],
    standalone: true,
    imports: [NgIf],
})
export class AlertComponent implements OnInit {
    @Input() alert: Alert;

    public AlertContext: AlertContext;

    constructor(private alertService: AlertService) {}

    ngOnInit(): void {}

    alertClass(): string {
        switch (this.alert.context) {
            case AlertContext.Danger:
                return "alert-danger";
            case AlertContext.Info:
                return "alert-info";
            case AlertContext.Primary:
                return "alert-primary";
            case AlertContext.Secondary:
                return "alert-secondary";
            case AlertContext.Success:
                return "alert-success";
            case AlertContext.Warning:
                return "alert-warning";
            default:
                return "";
        }
    }

    closeAlert(): void {
        this.alertService.removeAlert(this.alert);
    }
}
