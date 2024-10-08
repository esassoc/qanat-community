import { createApplication } from "@angular/platform-browser";
import { appConfig } from "./app/app.config";
import { createCustomElement } from "@angular/elements";
import { AppComponent } from "./app/app.component";
import { WaterAccountPopupComponent } from "./app/shared/components/maps/water-account-popup/water-account-popup.component";
import { ParcelPopupComponent } from "./app/shared/components/maps/parcel-popup/parcel-popup.component";
import { WellPopupComponent } from "./app/shared/components/maps/well-popup/well-popup.component";
import { UsageEntityPopupComponent } from "./app/shared/components/maps/usage-entity-popup/usage-entity-popup.component";

(async () => {
    const app = createApplication(appConfig);
    (await app).bootstrap(AppComponent);

    const ngElement = createCustomElement(WaterAccountPopupComponent, {
        injector: (await app).injector,
    });
    customElements.define("water-account-popup-custom-element", ngElement);

    const ngParcelElement = createCustomElement(ParcelPopupComponent, {
        injector: (await app).injector,
    });
    customElements.define("parcel-popup-custom-element", ngParcelElement);

    const ngWellElement = createCustomElement(WellPopupComponent, {
        injector: (await app).injector,
    });
    customElements.define("well-popup-custom-element", ngWellElement);

    const ngUsageEntityElement = createCustomElement(UsageEntityPopupComponent, {
        injector: (await app).injector,
    });
    customElements.define("usage-entity-popup-custom-element", ngUsageEntityElement);
})();
