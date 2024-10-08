import { LogLevel, Configuration, BrowserCacheLocation } from "@azure/msal-browser";

const isIE = window.navigator.userAgent.indexOf("MSIE ") > -1 || window.navigator.userAgent.indexOf("Trident/") > -1;

//Fill this out using your b2c values.
export const b2cPolicies = {
    names: {
        signUpSignIn: "signUpSignIn",
        editProfile: "editProfile",
        passwordReset: "passwordReset",
        signUp: "signUp",
        changeLogin: "changeLogin",
    },
    authorities: {
        signUpSignIn: {
            authority: "signUpSignIn",
        },
        signUp: {
            authority: "signUp",
        },
        editProfile: {
            authority: "editProfile",
        },
        passwordReset: {
            authority: "passwordReset",
        },
        changeLogin: {
            authority: "changeLogin",
        },
    },
    authorityDomain: "groundwateraccounting.b2clogin.com",
    apiScopes: ["https://groundwateraccounting.onmicrosoft.com/b1ff7cdb-46dc-4873-8d7c-5ba194208ab3/User.Access"],
};

/**
 * Configuration object to be passed to MSAL instance on creation.
 * For a full list of MSAL.js configuration parameters, visit:
 * https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-browser/docs/configuration.md
 */
export const msalConfig: Configuration = {
    auth: {
        //One ClientID per applicaiton, create this in the Azure B2C Page and update her for new apps.
        clientId: "clientId", // This is the ONLY mandatory field that you need to supply.
        authority: b2cPolicies.authorities.signUpSignIn.authority, // Defaults to "https://login.microsoftonline.com/common"
        knownAuthorities: [b2cPolicies.authorityDomain], // Mark your B2C tenant's domain as trusted.
        redirectUri: "/", // Points to window.location.origin. You must register this URI on Azure portal/App Registration.
        postLogoutRedirectUri: "/", // Indicates the page to navigate after logout.
        navigateToLoginRequestUrl: true, // If "true", will navigate back to the original request location before processing the auth code response.
    },
    cache: {
        cacheLocation: BrowserCacheLocation.LocalStorage, // Configures cache location. "sessionStorage" is more secure, but "localStorage" gives you SSO between tabs.
        storeAuthStateInCookie: true, // Set this to "true" if you are having issues on IE11 or Edge
    },
    system: {
        loggerOptions: {
            loggerCallback(logLevel: LogLevel, message: string) {
                // console.info(message);
            },
            logLevel: LogLevel.Verbose,
            piiLoggingEnabled: false,
        },
    },
};

/**
 * An optional silentRequest object can be used to achieve silent SSO
 * between applications by providing a "login_hint" property.
 */
export const silentRequest = {
    scopes: ["openid", "profile"],
    loginHint: "example@domain.net",
};
