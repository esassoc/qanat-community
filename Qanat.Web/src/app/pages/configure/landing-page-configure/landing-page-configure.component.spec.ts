import { ComponentFixture, TestBed } from "@angular/core/testing";

import { LandingPageConfigureComponent } from "./landing-page-configure.component";

describe("LandingPageConfigureComponent", () => {
    let component: LandingPageConfigureComponent;
    let fixture: ComponentFixture<LandingPageConfigureComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [LandingPageConfigureComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(LandingPageConfigureComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
