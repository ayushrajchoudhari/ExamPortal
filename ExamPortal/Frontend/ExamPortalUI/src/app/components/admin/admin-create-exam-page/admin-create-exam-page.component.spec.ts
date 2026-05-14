import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCreateExamComponent } from './admin-create-exam-page.component';

describe('AdminCreateExamComponent', () => {
  let component: AdminCreateExamComponent;
  let fixture: ComponentFixture<AdminCreateExamComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateExamComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateExamComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
