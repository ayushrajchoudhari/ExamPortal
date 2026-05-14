import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminExamSetSummaryDto } from '../../interfaces/admin-exam-set-summary-dto';
import { CreateExamRequestDto } from '../../interfaces/create-exam-request-dto';

@Injectable({
  providedIn: 'root'
})
export class AdminExamService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7178/api/admin/exams';

  // GetMyExams Endpoint
  public getMyExams(): Observable<Array<AdminExamSetSummaryDto>> {
    return this.http.get<Array<AdminExamSetSummaryDto>>(`${this.apiUrl}/get`);
  }

  // CreateExam Endpoint
  public createExam(payload: CreateExamRequestDto): Observable<{ examId: string, message: string }> {
    return this.http.post<{ examId: string, message: string }>(`${this.apiUrl}/create`, payload);
  }

  // DeleteExam Endpoint
  public deleteExam(examId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/delete/${examId}`);
  }
}