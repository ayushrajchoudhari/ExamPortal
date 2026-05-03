import { Component, OnInit, OnDestroy, signal, computed, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExamStateService } from '../../services/exams/state/exam-state.service';
import { AttemptService } from '../../services/attempt/attempt.service';
import { ActiveQuestionDto } from '../../interfaces/active-question-dto';
import { SubmitExamRequestDto } from '../../interfaces/submit-exam-request-dto';
import { SubmitExamResponseDto } from '../../interfaces/submit-exam-response-dto';

@Component({
  selector: 'app-exam-page',
  standalone: true,
  templateUrl: './exam-page.component.html',
  styleUrl: './exam-page.component.scss'
})
export class ExamPageComponent implements OnInit, OnDestroy {
  public state = inject(ExamStateService);
  private attemptService = inject(AttemptService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  private timerInterval: any;
  private currentAttemptId: string = '';

  // 1. Replaced mock data with an empty array of the strictly typed ActiveQuestionDto
  public questions = signal<ActiveQuestionDto[]>([]);
  public isLoading = signal<boolean>(true);

  public currentQuestion = computed(() => this.questions()[this.state.currentQuestionIndex()]);
  public isFirst = computed(() => this.state.currentQuestionIndex() === 0);
  public isLast = computed(() => this.state.currentQuestionIndex() === (this.questions().length - 1));

  ngOnInit() {
    this.currentAttemptId = this.route.snapshot.paramMap.get('attemptId') || '';
    
    if (this.currentAttemptId) {
      this.loadQuestionsFromServer();
    }
  }

  private loadQuestionsFromServer() {
    this.attemptService.getQuestions(this.currentAttemptId).subscribe({
      next: (data) => {
        // Double-cast safely bypasses the strict compiler mismatch
        const fetchedQuestions = (data as unknown) as Array<ActiveQuestionDto>;
        this.questions.set(fetchedQuestions? fetchedQuestions :new Array<ActiveQuestionDto>()); 
        this.isLoading.set(false);

        this.state.initExam(this.currentAttemptId, 60);

        this.timerInterval = setInterval(() => {
          this.state.decrementTime();
          if (this.state.isExamComplete()) {
             this.submitExam();
          }
        }, 1000);
      },
      error: (err) => {
        alert('Could not load questions. Are you authorized?');
        this.router.navigate(['/home']);
      }
    });
  }

  ngOnDestroy() {
    clearInterval(this.timerInterval);
  }

  public selectOption(optionId: string) {
    if (this.currentQuestion()) {
      this.state.selectAnswer(this.currentQuestion().id, optionId);
    }
  }

  public next() {
    if (!this.isLast()) this.state.currentQuestionIndex.update(i => i + 1);
  }

  public prev() {
    if (!this.isFirst()) this.state.currentQuestionIndex.update(i => i - 1);
  }

  public goTo(index: number) {
    this.state.currentQuestionIndex.set(index);
  }

  public submitExam() {
    clearInterval(this.timerInterval);
    const result = confirm('Are you sure you want to submit your exam?');
    
    if (result) {
      // 2. Map the state to the SubmitExamRequestDto expected by the C# Backend
      const payload = { answers: this.state.userAnswers() };

      this.attemptService.submitExam(this.currentAttemptId, payload).subscribe({
        next: (response) => {
          this.state.clearSession(); // Wipes localStorage so they can't re-enter the exam
          alert(`Exam Submitted! You scored ${response.totalScore} out of ${response.maxScore}. Passed: ${response.passed}`);
          // Next milestone: Route to a beautiful Results Page!
          this.router.navigate(['/result', this.currentAttemptId]);
        },
        error: (err) => {
          alert('Submission failed: ' + err.message);
        }
      });
    } else {
      // Resume timer if they cancel
      this.timerInterval = setInterval(() => this.state.decrementTime(), 1000);
    }
  }
}