import { TestBed } from '@angular/core/testing';

import { AdminExamService } from './admin-exam.service';

describe('AdminExamService', () => {
  let service: AdminExamService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AdminExamService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
