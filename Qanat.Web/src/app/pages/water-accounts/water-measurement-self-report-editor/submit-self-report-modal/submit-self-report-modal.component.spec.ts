import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubmitSelfReportModalComponent } from './submit-self-report-modal.component';

describe('SubmitSelfReportModalComponent', () => {
  let component: SubmitSelfReportModalComponent;
  let fixture: ComponentFixture<SubmitSelfReportModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubmitSelfReportModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SubmitSelfReportModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
