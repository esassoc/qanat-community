import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ReportingPeriodSelectComponent } from "./reporting-period-select.component";

describe("ReportingPeriodSelectComponent", () => {
    let component: ReportingPeriodSelectComponent;
    let fixture: ComponentFixture<ReportingPeriodSelectComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ReportingPeriodSelectComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(ReportingPeriodSelectComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
