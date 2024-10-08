import { Injectable, Injector } from "@angular/core";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { BusyService } from ".";
import { Alert } from "../models/alert";
import { AlertService } from "./alert.service";
import { Router } from "@angular/router";
import { datadogLogs } from "@datadog/browser-logs";

@Injectable({
    providedIn: "root",
})
export class GlobalErrorHandlerService {
    private busyService: BusyService;
    private alertService: AlertService;

    constructor(
        private injector: Injector,
        private authenticationService: AuthenticationService,
        private router: Router
    ) {
        this.busyService = this.injector.get(BusyService);
        this.alertService = this.injector.get(AlertService);
    }

    handleError(error: any) {
        if (
            error &&
            error.status !== 401 && // Unauthorized
            error.status !== 403 && // Forbidden
            error.status !== 404 && // Not Found (can easily happen when looking for a unexisting .po file)
            (error.message || "").indexOf("ViewDestroyedError: Attempt to use a destroyed view: detectChanges") < 0 && // issue in the ngx-loading package...waiting for it to be updated.
            (error.message || "").indexOf("ExpressionChangedAfterItHasBeenCheckedError") < 0 && // this only happens in dev angular build - I'm sure
            (error.message || "").indexOf("Loading chunk") < 0 && // also ignore loading chunk errors as they're handled in app.component NavigationError event
            (error.message || "").indexOf("<path> attribute d: Expected number,") < 0 // attrTween.js error related to charts
        ) {
            // IE Bug
            if ((error.message || "").indexOf("available to complete this operation.") >= 0) {
                this.alertService.pushAlert(new Alert(`Internet Explorer Error: ${error.message}`));
            }

            // catch that the user is trying to reset the password and redirect to password reset workflow
            // this was needed with the switch to the custom policy for Azure AD B2C
            if (error.errorMessage && error.errorMessage.includes("AADB2C90118")) {
                this.authenticationService.resetPassword();
            }
            if (error.errorMessage && error.errorMessage.includes("AADB2C90091")) {
                this.authenticationService.login();
                return;
            }
            console.error(error);

            if (error.stack) {
                datadogLogs.logger.error(error.message, error, error);
            }
        } else if (error) {
            console.warn(error);
            this.busyService.setBusy(false);
            datadogLogs.logger.warn(error);
        }
    }
}
