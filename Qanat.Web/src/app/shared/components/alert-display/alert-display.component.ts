import { Component, OnDestroy, OnInit, ChangeDetectorRef, Input } from "@angular/core";
import { AlertService } from "../../services/alert.service";
import { Alert } from "../../models/alert";
import { Subscription } from "rxjs";
import { AlertComponent } from "../alert/alert.component";
import { NgIf, NgFor } from "@angular/common";

@Component({
    selector: "app-alert-display",
    templateUrl: "./alert-display.component.html",
    styleUrls: ["./alert-display.component.css"],
    standalone: true,
    imports: [NgIf, NgFor, AlertComponent],
})
export class AlertDisplayComponent implements OnInit, OnDestroy {
    @Input() clearAlertsOnDestroy: boolean = true;
    public alerts: Alert[] = [];
    private alertSubscription: Subscription;

    constructor(
        private alertService: AlertService,
        private cdr: ChangeDetectorRef
    ) {}

    public ngOnInit(): void {
        this.alertSubscription = this.alertService.alertSubject.asObservable().subscribe((alerts) => {
            this.alerts = alerts;
            this.cdr.detectChanges();
        });
    }

    public ngOnDestroy(): void {
        this.alerts = null;
        this.alertSubscription.unsubscribe();
        if (this.clearAlertsOnDestroy) {
            this.alertService.clearAlerts();
        }
    }

    public closeAlert(alert: Alert) {
        this.alertService.removeAlert(alert);
    }
}
