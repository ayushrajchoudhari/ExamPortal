import { Component, OnInit, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ExamService, ExamSetSummaryDto } from '../../services/exams/exam/exam.service';
import { AuthService } from '../../core/services/auth/auth.service';

@Component({
  selector: 'app-home-page',
  standalone: true,
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
})
export class HomePageComponent implements OnInit {
  private examService = inject(ExamService);
  private router = inject(Router);

  // Inject AuthService as public so the HTML template can access its signals
  public authService = inject(AuthService);

  // UI State Signals
  public exams = signal<ExamSetSummaryDto[]>([]);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string | null>(null);

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

  public viewInstructions(examId: string): void {
    // We will build this route next
    this.router.navigate(['/user/instructions', examId]);
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