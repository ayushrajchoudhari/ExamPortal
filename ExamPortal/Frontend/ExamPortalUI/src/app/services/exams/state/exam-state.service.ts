



import { Injectable, signal, computed, effect } from '@angular/core';

export interface ExamSessionState {
  examId: string;
  timeRemaining: number;
  answers: Record<string, string>; // Maps questionId to selectedOptionId
}

@Injectable({
  providedIn: 'root',
})

export class ExamStateService {
  // Writable Signals
  public examId = signal<string | null>(null);
  public timeRemaining = signal<number>(0);
  public userAnswers = signal<Record<string, string>>({});
  public currentQuestionIndex = signal<number>(0);

  // Computed Signals (Read-only UI derivatives)
  public formattedTime = computed(() => {
    const total = this.timeRemaining();
    const m = Math.floor(total / 60).toString().padStart(2, '0');
    const s = (total % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
  });

  public isExamComplete = computed(() => this.timeRemaining() <= 0);

  constructor() {
    // 1. Hydrate state from localStorage on application load
    if (typeof window!== 'undefined') {
      const saved = localStorage.getItem('exam_session');
      if (saved) {
        const parsed = JSON.parse(saved) as ExamSessionState;
        this.examId.set(parsed.examId);
        this.timeRemaining.set(parsed.timeRemaining);
        this.userAnswers.set(parsed.answers);
      }
    }

    // 2. Automatically sync to localStorage whenever state changes
    effect(() => {
      const id = this.examId();
      if (id) {
        const state: ExamSessionState = {
          examId: id,
          timeRemaining: this.timeRemaining(),
          answers: this.userAnswers()
        };
        localStorage.setItem('exam_session', JSON.stringify(state));
      } else {
        localStorage.removeItem('exam_session');
      }
    });
  }

  // Action Methods
  public initExam(id: string, durationMinutes: number) {
    // Only reset if it's a completely new exam attempt
    if (this.examId()!== id) {
      this.examId.set(id);
      this.timeRemaining.set(durationMinutes * 60);
      this.userAnswers.set({});
      this.currentQuestionIndex.set(0);
    }
  }

  public selectAnswer(questionId: string, optionId: string) {
    this.userAnswers.update(prev => ({...prev, [questionId]: optionId }));
  }

  public decrementTime() {
    if (this.timeRemaining() > 0) {
      this.timeRemaining.update(t => t - 1);
    }
  }

  public clearSession() {
    this.examId.set(null);
  }
}