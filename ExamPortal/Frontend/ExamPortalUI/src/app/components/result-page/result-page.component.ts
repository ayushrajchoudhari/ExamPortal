import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AttemptService } from '../../services/attempt/attempt.service';
import { ExamResultDto } from '../../interfaces/exam-result-dto';
import { DatePipe, DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-result-page',
  standalone: true,
  imports: [DatePipe, DecimalPipe],
  templateUrl: './result-page.component.html',
  styleUrl: './result-page.component.scss'
})
export class ResultPageComponent implements OnInit {
  private attemptService = inject(AttemptService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  // UI State Signals
  public result = signal<ExamResultDto | null>(null);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string | null>(null);

  ngOnInit() {
    const attemptId = this.route.snapshot.paramMap.get('attemptId');
    if (attemptId) {
      this.fetchResult(attemptId);
    } else {
      this.router.navigate(['/home']);
    }
  }

  private fetchResult(attemptId: string) {
    this.attemptService.getResult(attemptId).subscribe({
      next: (data) => {
        this.result.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Failed to load exam results.');
        this.isLoading.set(false);
      }
    });
  }

  public goHome() {
    this.router.navigate(['/home']);
  }
}