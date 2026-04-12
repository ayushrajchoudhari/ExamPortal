import { Component, OnInit, OnDestroy, signal, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ExamStateService } from '../../services/exams/state/exam-state.service';

@Component({
  selector: 'app-exam-page',
  standalone: true,
  templateUrl: './exam-page.component.html',
  styleUrl: './exam-page.component.scss'
})
export class ExamPageComponent implements OnInit, OnDestroy {
  public state = inject(ExamStateService);
  private router = inject(Router);
  private timerInterval: any;

  // Temporary Mock Data for UI Testing
  public questions = signal<any>([
    { id: 'q1', text: 'What is the default DI lifetime for services in Angular?', points: 10, options: [{id: 'o1', text: 'Singleton'}, {id: 'o2', text: 'Transient'}, {id: 'o3', text: 'Scoped'}] },
    { id: 'q2', text: 'What is the recommended DI lifetime for DbContext?', points: 10, options: []},
    { id: 'q3', text: 'Which Angular API handles synchronous state reactivity?', points: 10, options: []},
    { id: 'q4', text: 'Which index determines the physical sorting of a table?', points: 10, options: [{id: 'o10', text: 'Clustered Index'}, {id: 'o11', text: 'Non-Clustered Index'}, {id: 'o12', text: 'Hash Index'}] },
    { id: 'q5', text: 'How do you remove Zone.js in Angular 20?', points: 10, options: []}
  ]);

  public currentQuestion = computed(() => this.questions()[this.state.currentQuestionIndex()]);
  public isFirst = computed(() => this.state.currentQuestionIndex() === 0);
  public isLast = computed(() => this.state.currentQuestionIndex() === this.questions().length - 1);

  ngOnInit() {
    // Initialize exam with 60 minutes. If refreshed, the service will ignore this and use localStorage state.
    this.state.initExam('exam-123', 60);

    this.timerInterval = setInterval(() => {
      this.state.decrementTime();
      if (this.state.isExamComplete()) {
         this.submitExam();
      }
    }, 1000);
  }

  ngOnDestroy() {
    clearInterval(this.timerInterval);
  }

  public selectOption(optionId: string) {
    this.state.selectAnswer(this.currentQuestion().id, optionId);
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
      alert('Exam Submitted successfully!\n\nPayload: ' + JSON.stringify(this.state.userAnswers()));
      this.state.clearSession();
      this.router.navigate(['/home']);
    } else {
      // Resume timer if they cancel
      this.timerInterval = setInterval(() => this.state.decrementTime(), 1000);
    }
  }
}