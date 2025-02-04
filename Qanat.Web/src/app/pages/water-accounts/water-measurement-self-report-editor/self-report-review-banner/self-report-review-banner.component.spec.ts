import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelfReportReviewBannerComponent } from './self-report-review-banner.component';

describe('SelfReportReviewBannerComponent', () => {
  let component: SelfReportReviewBannerComponent;
  let fixture: ComponentFixture<SelfReportReviewBannerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelfReportReviewBannerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SelfReportReviewBannerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
