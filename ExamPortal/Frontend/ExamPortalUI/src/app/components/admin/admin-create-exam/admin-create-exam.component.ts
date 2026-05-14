import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminExamService } from '../../../services/admin/admin-exam.service';
import { CreateExamRequestDto } from '../../../interfaces/create-exam-request-dto';

@Component({
  selector: 'app-admin-create-exam',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin-create-exam.component.html',
  styleUrl: './admin-create-exam.component.scss'
})
export class AdminCreateExamComponent {
  private adminExamService = inject(AdminExamService);
  private router = inject(Router);

  // Form State Signal
  public formData = signal<CreateExamRequestDto>({
    title: '',
    durationMinutes: 60,
    passingScore: 70,
    instructionContent: '',
    isPublic: true
  });

  // UI State Signals
  public isLoading = signal<boolean>(false);
  public errorMessage = signal<string | null>(null);

  public onSubmit() {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.adminExamService.createExam(this.formData()).subscribe({
      next: (response) => {
        this.isLoading.set(false);
        // Route back to dashboard, where the new exam will appear in the table
        this.router.navigate(['/admin/home']);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to create the exam. Please try again.');
      }
    });
  }

  public cancel() {
    this.router.navigate(['/admin/home']);
  }
}