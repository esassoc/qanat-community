import { ErrorHandler, ApplicationConfig, importProvidersFrom, provideAppInitializer, inject } from "@angular/core";
import { HTTP_INTERCEPTORS, provideHttpClient, withFetch, withInterceptorsFromDi } from "@angular/common/http";
import { RouterModule, TitleStrategy, provideRouter } from "@angular/router";
import { DecimalPipe, CurrencyPipe, DatePipe, AsyncPipe, KeyValuePipe, PercentPipe } from "@angular/common";
import { HttpErrorInterceptor } from "./shared/interceptors/httpErrorInterceptor";
import { environment } from "src/environments/environment";
import { GlobalErrorHandlerService } from "./shared/services/global-error-handler.service";
import { IPublicClientApplication, PublicClientApplication, InteractionType } from "@azure/msal-browser";
import {
    MsalGuard,
    MsalBroadcastService,
    MsalService,
    MSAL_GUARD_CONFIG,
    MSAL_INSTANCE,
    MsalGuardConfiguration,
    MsalInterceptorConfiguration,
    MSAL_INTERCEPTOR_CONFIG,
    MsalInterceptor,
    ProtectedResourceScopes,
} from "@azure/msal-angular";
import { b2cPolicies, msalConfig } from "./auth.config";
import { PageTitleStrategy } from "./strategies/page-title-strategy";
import { DatadogService } from "./shared/services/datadog.service";
import { provideAnimations } from "@angular/platform-browser/animations";
import { ApiModule } from "./shared/generated/api.module";
import { Configuration } from "./shared/generated/configuration";
import { routes } from "./app.routes";
import { PhonePipe } from "./shared/pipes/phone.pipe";
import { GroupByPipe } from "./shared/pipes/group-by.pipe";
import { TimeAgoPipe } from "./shared/pipes/time-ago.pipe";
import { ZipCodePipe } from "./shared/pipes/zipcode.pipe";
import { SumPipe } from "./shared/pipes/sum.pipe";
import { TinyMceConfigPipe } from "./shared/pipes/tiny-mce-config.pipe";
import { RequiredPipe } from "./shared/pipes/required.pipe";
import { CommaJoinPipe } from "./shared/pipes/comma-join.pipe";
import { MapPipe } from "./shared/pipes/map.pipe";
import { provideDialogConfig } from "@ngneat/dialog";
import { RECAPTCHA_V3_SITE_KEY } from "ng-recaptcha-2";

export function init_app(datadogService: DatadogService) {
    return () => {
        if (environment.datadogClientToken) {
            datadogService.init();
        }
    };
}

/**
 * Here we pass the configuration parameters to create an MSAL instance.
 * For more info, visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-angular/docs/v2-docs/configuration.md
 */
export function MSALInstanceFactory(): IPublicClientApplication {
    return new PublicClientApplication(msalConfig);
}

/**
 * Set your default interaction type for MSALGuard here. If you have any
 * additional scopes you want the user to consent upon login, add them here as well.
 */
export function MSALGuardConfigFactory(): MsalGuardConfiguration {
    return {
        interactionType: InteractionType.Redirect,
    };
}

export function MSALInterceptorConfigFactory(): MsalInterceptorConfiguration {
    const protectedResourceMap = new Map<string, Array<string | ProtectedResourceScopes> | null>([
        [`${environment.mainAppApiUrl}/public/*`, [{ httpMethod: "GET", scopes: null }]],
        [`${environment.mainAppApiUrl}/public/support-tickets`, [{ httpMethod: "POST", scopes: null }]],
        [`${environment.mainAppApiUrl}/*`, b2cPolicies.apiScopes],
    ]);

    return {
        interactionType: InteractionType.Redirect,
        protectedResourceMap,

        authRequest: (msalService, httpReq, originalAuthRequest) => {
            const accounts = msalService.instance.getAllAccounts();

            if (accounts && accounts.length > 0) {
                protectedResourceMap.set(`${environment.mainAppApiUrl}/public/*`, b2cPolicies.apiScopes);
            }

            return originalAuthRequest;
        },
    };
}

export function initializeMsal(pca: IPublicClientApplication) {
    return () => pca.initialize();
}

export const appConfig: ApplicationConfig = {
    providers: [
        provideRouter(routes),
        { provide: RECAPTCHA_V3_SITE_KEY, useValue: environment.recaptchaV3SiteKey },
        importProvidersFrom(
            ApiModule.forRoot(() => {
                return new Configuration({
                    basePath: `${environment.mainAppApiUrl}`,
                });
            })
        ),
        importProvidersFrom(
            RouterModule.forRoot(routes, {
                paramsInheritanceStrategy: "always",
                scrollPositionRestoration: "enabled",
                anchorScrolling: "enabled",
            })
        ),
        provideHttpClient(withInterceptorsFromDi(), withFetch()),
        provideAnimations(),
        { provide: HTTP_INTERCEPTORS, useClass: MsalInterceptor, multi: true },
        {
            provide: MSAL_INSTANCE,
            useFactory: MSALInstanceFactory,
        },
        {
            provide: MSAL_GUARD_CONFIG,
            useFactory: MSALGuardConfigFactory,
        },
        {
            provide: MSAL_INTERCEPTOR_CONFIG,
            useFactory: MSALInterceptorConfigFactory,
        },
        { provide: TitleStrategy, useClass: PageTitleStrategy },
        MsalService,
        MsalGuard,
        MsalBroadcastService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: HttpErrorInterceptor,
            multi: true,
        },
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandlerService,
        },
        DecimalPipe,
        CurrencyPipe,
        DatePipe,
        PhonePipe,
        GroupByPipe,
        AsyncPipe,
        TimeAgoPipe,
        ZipCodePipe,
        KeyValuePipe,
        SumPipe,
        PercentPipe,
        TinyMceConfigPipe,
        RequiredPipe,
        CommaJoinPipe,
        MapPipe,
        provideDialogConfig({
            sizes: {
                sm: {
                    width: "100%",
                    maxWidth: "540px",
                    maxHeight: "90vh",
                },
                lg: {
                    width: "100%",
                    maxWidth: "1280px",
                    maxHeight: "90vh",
                },
            },
        }),
        provideAppInitializer(() => {
            const pca = inject<IPublicClientApplication>(MSAL_INSTANCE);
            return pca.initialize();
        }),
    ],
};
