import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminExamReportPageComponent } from './admin-exam-report-page.component';

describe('AdminExamReportPageComponent', () => {
  let component: AdminExamReportPageComponent;
  let fixture: ComponentFixture<AdminExamReportPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminExamReportPageComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminExamReportPageComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
