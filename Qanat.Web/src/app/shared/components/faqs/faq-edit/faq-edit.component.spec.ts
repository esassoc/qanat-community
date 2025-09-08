import { ComponentFixture, TestBed } from "@angular/core/testing";

import { FaqEditComponent } from "./faq-edit.component";

describe("FaqEditComponent", () => {
    let component: FaqEditComponent;
    let fixture: ComponentFixture<FaqEditComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [FaqEditComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(FaqEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
