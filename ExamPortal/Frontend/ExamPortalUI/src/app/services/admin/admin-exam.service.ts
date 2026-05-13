import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminExamSetSummaryDto } from '../../interfaces/admin-exam-set-summary-dto';

@Injectable({
  providedIn: 'root'
})
export class AdminExamService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7178/api/admin/exams';

  public getMyExams(): Observable<Array<AdminExamSetSummaryDto>> {
    return this.http.get<Array<AdminExamSetSummaryDto>>(`${this.apiUrl}/get`);
  }

  // Placeholder for the upcoming Delete backend endpoint
  public deleteExam(examId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete/${examId}`);
  }
}