import { Component, OnInit, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ExamService } from '../../services/exams/exam/exam.service';
import { AuthService } from '../../core/services/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { AttemptService } from '../../services/attempt/attempt.service';
import { ExamSetSummaryDto } from '../../interfaces/exam-set-summary-dto';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
})
export class HomePageComponent implements OnInit {
  private examService = inject(ExamService);
  private attemptService = inject(AttemptService);
  private router = inject(Router);

  // Inject AuthService as public so the HTML template can access its signals
  public authService = inject(AuthService);

  // UI State Signals
  public exams = signal<ExamSetSummaryDto[]>([]);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string | null>(null);

  // Modal State Signals
  public selectedPrivateExamId = signal<string | null>(null);
  public secretTokenInput = signal<string>('');
  public tokenError = signal<string | null>(null);
  public isValidating = signal<boolean>(false);

  ngOnInit(): void {
    this.fetchExams();
  }

  private fetchExams(): void {
    this.examService.getAvailableExams().subscribe({
      next: (data) => {
        this.exams.set(data ? data : []);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage.set('Failed to load exams. Ensure your.NET API is running and CORS is configured.');
        this.isLoading.set(false);
      }
    });
  }

  public viewInstructions(examId: string, isPublic: boolean): void {
    if (isPublic) {
      this.router.navigate(['/user/instructions', examId]);
    } else {
      this.selectedPrivateExamId.set(examId);
      this.secretTokenInput.set('');
      this.tokenError.set(null);
    }
  }

  public submitToken(): void {
    const examId = this.selectedPrivateExamId();
    if (!examId) return;

    this.isValidating.set(true);
    this.tokenError.set(null);

    this.attemptService.validateToken({ examSetId: examId, token: this.secretTokenInput() }).subscribe({
      next: () => {
        this.isValidating.set(false);
        this.selectedPrivateExamId.set(null);
        this.router.navigate(['/user/instructions', examId]);
      },
      error: (err) => {
        this.isValidating.set(false);
        this.tokenError.set(err.error?.message || 'Invalid Token');
      }
    });
  }

  public cancelToken(): void {
    this.selectedPrivateExamId.set(null);
    this.secretTokenInput.set('');
    this.tokenError.set(null);
  }

  // Navigation action for the header button
  public navigateToLogin(): void {
    this.router.navigate(['/login']);
  }

  // Logout action for the header button
  public logout(): void {
    this.authService.logout();
  }
}