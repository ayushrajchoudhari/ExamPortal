import { Component, OnInit, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminExamService } from '../../../services/admin/admin-exam.service';
import { AdminExamDetailsDto } from '../../../interfaces/admin-exam-details-dto';

@Component({
  selector: 'app-admin-edit-exam',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin-edit-exam-page.component.html',
  styleUrl: './admin-edit-exam-page.component.scss'
})
export class AdminEditExamPageComponent implements OnInit {
  private adminExamService = inject(AdminExamService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  public examId = signal<string>('');
  public formData = signal<AdminExamDetailsDto | null>(null);
  public isLoading = signal<boolean>(true);
  public isSaving = signal<boolean>(false);
  public errorMessage = signal<string | null>(null);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.examId.set(id);
      this.loadExamDetails();
    } else {
      this.router.navigate(['/admin/home']);
    }
  }

  private loadExamDetails() {
    this.adminExamService.getExamById(this.examId()).subscribe({
      next: (data) => {
        this.formData.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Failed to load exam details.');
        this.isLoading.set(false);
      }
    });
  }

  // --- Array Manipulation Methods ---
  public addQuestion() {
    this.formData.update(data => {
      if (!data) return data;
      data.questions.push({
        id: null,
        questionText: '',
        points: 10,
        displayOrder: data.questions.length + 1,
        options: []
      });
      return {...data };
    });
  }

  public removeQuestion(qIndex: number) {
    this.formData.update(data => {
      if (!data) return data;
      data.questions.splice(qIndex, 1);
      // Re-adjust display orders
      data.questions.forEach((q, idx) => q.displayOrder = idx + 1);
      return {...data };
    });
  }

  public addOption(qIndex: number) {
    this.formData.update(data => {
      if (!data) return data;
      data.questions[qIndex].options.push({
        id: null,
        optionText: '',
        isCorrect: false
      });
      return {...data };
    });
  }

  public removeOption(qIndex: number, oIndex: number) {
    this.formData.update(data => {
      if (!data) return data;
      data.questions[qIndex].options.splice(oIndex, 1);
      return {...data };
    });
  }

  public markOptionCorrect(qIndex: number, oIndex: number) {
    this.formData.update(data => {
      if (!data) return data;
      // Ensure only one option is marked correct per question
      data.questions[qIndex].options.forEach((opt, idx) => {
        opt.isCorrect = (idx === oIndex);
      });
      return {...data };
    });
  }

  // --- Submission ---
  public onSubmit() {
    if (!this.formData()) return;
    
    this.isSaving.set(true);
    this.errorMessage.set(null);

    this.adminExamService.updateExam(this.examId(), this.formData()!).subscribe({
      next: () => {
        this.isSaving.set(false);
        alert('Exam saved successfully!');
        this.router.navigate(['/admin/home']);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to update exam.');
      }
    });
  }

  public cancel() {
    this.router.navigate(['/admin/home']);
  }
}