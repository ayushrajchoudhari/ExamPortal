import { Component, OnInit, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AdminExamService } from '../../../services/admin/admin-exam.service';
import { AuthService } from '../../../core/services/auth/auth.service';
import { AdminExamSetSummaryDto } from '../../../interfaces/admin-exam-set-summary-dto';

@Component({
  selector: 'app-admin-home',
  standalone: true,
  templateUrl: './admin-home-page.component.html',
  styleUrl: './admin-home-page.component.scss'
})
export class AdminHomePageComponent implements OnInit {
  private adminExamService = inject(AdminExamService);
  public authService = inject(AuthService);
  private router = inject(Router);

  public exams = signal<Array<AdminExamSetSummaryDto>>(new Array<AdminExamSetSummaryDto>());
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string | null>(null);

  ngOnInit() {
    this.fetchAdminExams();
  }

  private fetchAdminExams() {
    this.adminExamService.getMyExams().subscribe({
      next: (data) => {
        const fetchedExams = data as Array<AdminExamSetSummaryDto>;
        this.exams.set(fetchedExams? fetchedExams : new Array<AdminExamSetSummaryDto>());
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Failed to load admin dashboard data.');
        this.isLoading.set(false);
      }
    });
  }

  public createNewExam() {
    this.router.navigate(['/admin/exams/createExam']);
  }

  public editExam(examId: string) {
    this.router.navigate(['/admin/exams/editExam', examId]);
  }

  public deleteExam(examId: string) {
    const confirmDelete = confirm('Are you sure you want to permanently delete this ExamSet? This action cannot be undone.');
    
    if (confirmDelete) {
      this.exams.update(currentExams => currentExams.filter(e => e.id!== examId));
      
      this.adminExamService.deleteExam(examId).subscribe({
        error: () => alert('Failed to delete the exam on the server.')
      });
    }
  }

  public logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}