import { ComponentFixture, TestBed } from "@angular/core/testing";

import { UpdateMeterModalComponent } from "./update-meter-modal.component";

describe("UpdateMeterModalComponent", () => {
    let component: UpdateMeterModalComponent;
    let fixture: ComponentFixture<UpdateMeterModalComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [UpdateMeterModalComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(UpdateMeterModalComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
