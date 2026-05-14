import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminEditExamPageComponent } from './admin-edit-exam-page.component';

describe('AdminEditExamPageComponent', () => {
  let component: AdminEditExamPageComponent;
  let fixture: ComponentFixture<AdminEditExamPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminEditExamPageComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminEditExamPageComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
