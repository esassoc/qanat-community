import { ComponentFixture, TestBed } from "@angular/core/testing";

import { EditFaqModalComponent } from "./edit-faq-modal.component";

describe("EditFaqModalComponent", () => {
    let component: EditFaqModalComponent;
    let fixture: ComponentFixture<EditFaqModalComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [EditFaqModalComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(EditFaqModalComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
