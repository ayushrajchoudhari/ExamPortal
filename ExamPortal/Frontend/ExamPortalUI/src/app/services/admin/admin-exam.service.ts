import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminExamSetSummaryDto } from '../../interfaces/admin-exam-set-summary-dto';
import { CreateExamRequestDto } from '../../interfaces/create-exam-request-dto';
import { AdminExamDetailsDto } from '../../interfaces/admin-exam-details-dto';
import { AdminExamReportDto } from '../../interfaces/admin-exam-report-dto';

@Injectable({
  providedIn: 'root'
})
export class AdminExamService {
  private http = inject(HttpClient);
  private readonly apiUrl = 'https://localhost:7178/api/admin/exams';

  // GetMyExams Endpoint
  public getMyExams(): Observable<Array<AdminExamSetSummaryDto>> {
    return this.http.get<Array<AdminExamSetSummaryDto>>(`${this.apiUrl}/GetMyExams`);
  }

  // CreateExam Endpoint
  public createExam(payload: CreateExamRequestDto): Observable<{ examId: string, message: string }> {
    return this.http.post<{ examId: string, message: string }>(`${this.apiUrl}/CreateExam`, payload);
  }

  // DeleteExam Endpoint
  public deleteExam(examId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/DeleteExamById/${examId}`);
  }

  // GetExamById Endpoint
  public getExamById(examId: string): Observable<AdminExamDetailsDto> {
    return this.http.get<AdminExamDetailsDto>(`${this.apiUrl}/GetExamById/${examId}`);
  }

  // UpdateExamById Endpoint
  public updateExam(examId: string, payload: AdminExamDetailsDto): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/UpdateExamById/${examId}`, payload);
  }

  // GetExamReport Endpoint
  public getExamReport(examId: string): Observable<AdminExamReportDto> {
    return this.http.get<AdminExamReportDto>(`${this.apiUrl}/${examId}/report`);
  }
}