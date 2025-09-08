import { ComponentFixture, TestBed } from "@angular/core/testing";

import { FaqDisplayEditModalComponent } from "./faq-display-edit-modal.component";

describe("FaqDisplayEditModalComponent", () => {
    let component: FaqDisplayEditModalComponent;
    let fixture: ComponentFixture<FaqDisplayEditModalComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [FaqDisplayEditModalComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(FaqDisplayEditModalComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
