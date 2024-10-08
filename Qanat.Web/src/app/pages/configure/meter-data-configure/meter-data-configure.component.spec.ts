import { ComponentFixture, TestBed } from "@angular/core/testing";

import { MeterDataConfigureComponent } from "./meter-data-configure.component";

describe("MeterDataConfigureComponent", () => {
    let component: MeterDataConfigureComponent;
    let fixture: ComponentFixture<MeterDataConfigureComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MeterDataConfigureComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(MeterDataConfigureComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
