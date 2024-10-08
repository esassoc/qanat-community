import { ComponentFixture, TestBed } from "@angular/core/testing";

import { FaqDisplayComponent } from "./faq-display.component";

describe("FaqDisplayComponent", () => {
    let component: FaqDisplayComponent;
    let fixture: ComponentFixture<FaqDisplayComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [FaqDisplayComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(FaqDisplayComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
