import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstructionPage } from './instruction-page.component';

describe('InstructionPage', () => {
  let component: InstructionPage;
  let fixture: ComponentFixture<InstructionPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InstructionPage],
    }).compileComponents();

    fixture = TestBed.createComponent(InstructionPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
