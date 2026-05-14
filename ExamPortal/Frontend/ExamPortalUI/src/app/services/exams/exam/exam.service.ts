import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExamSetSummaryDto } from '../../../interfaces/exam-set-summary-dto';

@Injectable({
  providedIn: 'root'
})
export class ExamService {
  private http = inject(HttpClient);
  
  // Ensure this port matches your.NET API launchSettings.json
  private readonly apiUrl = 'https://localhost:7178/api/exams';

  public getAvailableExams(): Observable<ExamSetSummaryDto[]> {
    return this.http.get<ExamSetSummaryDto[]>(`${this.apiUrl}/AvailableExams`);
  }
}