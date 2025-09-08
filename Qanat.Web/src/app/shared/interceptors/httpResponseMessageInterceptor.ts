import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from "@angular/common/http";

import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { Router } from "@angular/router";
import { AlertService } from "../services/alert.service";
import { AuthenticationService } from "src/app/shared/services/authentication.service";
import { Alert } from "../models/alert";
import { AlertContext } from "../models/enums/alert-context.enum";

@Injectable()
export class ResponseMessageInterceptor implements HttpInterceptor {
    constructor(
        private router: Router,
        private alertService: AlertService,
        private authenticationService: AuthenticationService
    ) {}
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            tap((x: any) => {
                if (this.isMessageResponseDto(x.body)) {
                    this.alertFromMessageResponseDto(x.body);
                }
            })
        );
    }

    private isMessageResponseDto(body: any): boolean {
        if (body instanceof Object) {
            const keys = Object.keys(body);
            return keys.includes("Messages");
        }
        return false;
    }

    private alertFromMessageResponseDto(body: any): void {
        body.Messages?.filter((message) => message.AlertMessageType === 0).forEach((message) => {
            this.alertService.pushAlert(new Alert(message.Message, AlertContext.Danger));
        });
        body.Messages?.filter((message) => message.AlertMessageType === 1).forEach((message) => {
            this.alertService.pushAlert(new Alert(message.Message, AlertContext.Success));
        });
        body.Messages?.filter((message) => message.AlertMessageType === 2).forEach((message) => {
            this.alertService.pushAlert(new Alert(message.Message, AlertContext.Warning));
        });
        body.Messages?.filter((message) => message.AlertMessageType === 3).forEach((message) => {
            this.alertService.pushAlert(new Alert(message.Message, AlertContext.Info));
        });
    }
}
