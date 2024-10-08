import { ComponentFixture, TestBed } from "@angular/core/testing";

import { AdminFrequentlyAskedQuestionsComponent } from "./admin-frequently-asked-questions.component";

describe("AdminFrequentlyAskedQuestionsComponent", () => {
    let component: AdminFrequentlyAskedQuestionsComponent;
    let fixture: ComponentFixture<AdminFrequentlyAskedQuestionsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AdminFrequentlyAskedQuestionsComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(AdminFrequentlyAskedQuestionsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it("should create", () => {
        expect(component).toBeTruthy();
    });
});
