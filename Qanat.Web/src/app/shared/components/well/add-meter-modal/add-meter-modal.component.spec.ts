import { ComponentFixture, TestBed } from "@angular/core/testing";

import { AddMeterModalComponent } from "./add-meter-modal.component";

describe("AddMeterModalComponent", () => {
    let component: AddMeterModalComponent;
    let fixture: ComponentFixture<AddMeterModalComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AddMeterModalComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AddMeterModalComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
