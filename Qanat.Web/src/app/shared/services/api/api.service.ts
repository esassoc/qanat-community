import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, throwError as _throw } from "rxjs";
import { AlertService } from "../alert.service";
import { Alert } from "src/app/shared/models/alert";
// import { OAuthService } from 'angular-oauth2-oidc';
import { AlertContext } from "../../models/enums/alert-context.enum";
import { MsalService } from "@azure/msal-angular";

@Injectable({
    providedIn: "root",
})
export class ApiService {
    constructor(
        private alertService: AlertService,
        private authService: MsalService,
        private router: Router
    ) {}
    // todo: We should refactor this handleError out of the ApiService and handle all http errors
    // with the httpErrorInterceptor class in the future
    public handleError(error: any, supressErrorMessage = false, clearBusyGlobally = true): Observable<any> {
        console.log(error);
        if (!supressErrorMessage) {
            if (error && error.status === 401) {
                this.router.navigate(["/"]).then((x) => {
                    this.alertService.pushAlert(new Alert("There was an error authorizing with the application.", AlertContext.Danger));
                });
            } else if (error && error.status === 403) {
                // letting the httpErrorInterceptor handle this now
            } else if (error.error && typeof error.error === "string") {
                this.alertService.pushAlert(new Alert(error.error, AlertContext.Danger, true));
            } else if (error.error && error.status === 404) {
                // let the caller handle not found appropriate to whatever it was doing
            } else if (error.error && !(error.error instanceof ProgressEvent)) {
                // FIXME: will break if error.error[key] or error.error.errors[key] is not a string[]
                if (error.error.errors) {
                    for (const key of Object.keys(error.error.errors)) {
                        const newLocal = new Alert(
                            (error.error.errors[key] as string[])
                                .map((fe: string) => {
                                    return key + ": " + fe;
                                })
                                .join(","),
                            AlertContext.Danger
                        );
                        this.alertService.pushAlert(newLocal);
                    }
                } else {
                    for (const key of Object.keys(error.error)) {
                        const newLocal = new Alert(
                            (error.error[key] as string[])
                                .map((fe: string) => {
                                    return key + ": " + fe;
                                })
                                .join(","),
                            AlertContext.Danger
                        );
                        this.alertService.pushAlert(newLocal);
                    }
                }
            } else {
                this.alertService.pushAlert(new Alert("Oops! Something went wrong and we couldn't complete the action..."));
            }
        }

        return _throw(error);
    }
}
