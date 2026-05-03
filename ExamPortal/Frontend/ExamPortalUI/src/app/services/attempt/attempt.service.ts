import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActiveQuestionDto } from '../../interfaces/active-question-dto';
import { SubmitExamRequestDto } from '../../interfaces/submit-exam-request-dto';
import { SubmitExamResponseDto } from '../../interfaces/submit-exam-response-dto';
import { StartAttemptResponseDto } from '../../interfaces/start-attempt-response-dto';
import { ExamResultDto } from '../../interfaces/exam-result-dto';

@Injectable({
  providedIn: 'root'
})
export class AttemptService {
  private http = inject(HttpClient);
  // Note: Using port 7178 based on your Swagger screenshots
  private readonly apiUrl = 'https://localhost:7178/api/attempts';

  public startAttempt(examSetId: string): Observable<StartAttemptResponseDto> {
    return this.http.post<StartAttemptResponseDto>(`${this.apiUrl}/start`, { examSetId });
  }

  public getQuestions(attemptId: string): Observable<ActiveQuestionDto> {
    return this.http.get<ActiveQuestionDto>(`${this.apiUrl}/${attemptId}/questions`);
  }

  public submitExam(attemptId: string, payload: SubmitExamRequestDto): Observable<SubmitExamResponseDto> {
    return this.http.post<SubmitExamResponseDto>(`${this.apiUrl}/${attemptId}/submit`, payload);
  }

  public getResult(attemptId: string): Observable<ExamResultDto> {
    return this.http.get<ExamResultDto>(`${this.apiUrl}/${attemptId}/result`);
  }
}