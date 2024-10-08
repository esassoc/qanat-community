import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ParcelDetailLayoutComponent } from "./parcel-detail-layout.component";

describe("ParcelDetailLayoutComponent", () => {
    let component: ParcelDetailLayoutComponent;
    let fixture: ComponentFixture<ParcelDetailLayoutComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ParcelDetailLayoutComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(ParcelDetailLayoutComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
