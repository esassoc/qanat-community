import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSupportModalComponent } from './create-support-modal.component';

describe('CreateSupportModalComponent', () => {
  let component: CreateSupportModalComponent;
  let fixture: ComponentFixture<CreateSupportModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateSupportModalComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CreateSupportModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
