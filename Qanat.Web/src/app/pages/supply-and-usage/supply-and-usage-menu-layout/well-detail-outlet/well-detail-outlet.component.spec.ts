import { ComponentFixture, TestBed } from "@angular/core/testing";

import { WellDetailOutletComponent } from "./well-detail-outlet.component";

describe("WellDetailOutletComponent", () => {
    let component: WellDetailOutletComponent;
    let fixture: ComponentFixture<WellDetailOutletComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [WellDetailOutletComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(WellDetailOutletComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
