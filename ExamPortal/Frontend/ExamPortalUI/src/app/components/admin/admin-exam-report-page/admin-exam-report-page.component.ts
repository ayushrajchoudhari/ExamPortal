import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { AdminExamService } from '../../../services/admin/admin-exam.service';
import { AdminExamReportDto } from '../../../interfaces/admin-exam-report-dto';

@Component({
  selector: 'app-admin-exam-report',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './admin-exam-report-page.component.html',
  styleUrl: './admin-exam-report-page.component.scss'
})
export class AdminExamReportPageComponent implements OnInit {
  private adminExamService = inject(AdminExamService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  public report = signal<AdminExamReportDto | null>(null);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string | null>(null);

  ngOnInit() {
    const examId = this.route.snapshot.paramMap.get('id');
    if (examId) {
      this.fetchReport(examId);
    } else {
      this.router.navigate(['/admin/home']);
    }
  }

  private fetchReport(examId: string) {
    this.adminExamService.getExamReport(examId).subscribe({
      next: (data) => {
        this.report.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Failed to load report.');
        this.isLoading.set(false);
      }
    });
  }

  public goBack() {
    this.router.navigate(['/admin/home']);
  }
}