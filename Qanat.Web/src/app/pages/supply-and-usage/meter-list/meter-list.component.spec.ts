import { ComponentFixture, TestBed } from "@angular/core/testing";

import { MeterListComponent } from "./meter-list.component";

describe("MeterListComponent", () => {
    let component: MeterListComponent;
    let fixture: ComponentFixture<MeterListComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MeterListComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(MeterListComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
