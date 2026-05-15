import { Routes } from '@angular/router';
import { InstructionPage } from './components/instruction-page/instruction-page.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { ExamPageComponent } from './components/exam-page/exam-page.component';
import { ResultPageComponent } from './components/result-page/result-page.component';
import { AdminHomePageComponent } from './components/admin/admin-home-page/admin-home-page.component';
import { AdminCreateExamPageComponent } from './components/admin/admin-create-exam-page/admin-create-exam-page.component';
import { AdminEditExamPageComponent } from './components/admin/admin-edit-exam-page/admin-edit-exam-page.component';
import { AdminExamReportPageComponent } from './components/admin/admin-exam-report-page/admin-exam-report-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'instructions', component: InstructionPage },
  { path: 'login', component: LoginPageComponent },
  { path: 'home', component: HomePageComponent },
  { path: 'exam', component: ExamPageComponent },
  { path: 'user/instructions/:id', component: InstructionPage },
  { path: 'examination/:attemptId', component: ExamPageComponent },
  { path: 'result/:attemptId', component: ResultPageComponent },
  { path: 'admin/home', component: AdminHomePageComponent },
  { path: 'admin/exams/createExam', component: AdminCreateExamPageComponent },
  { path: 'admin/exams/editExam/:id', component: AdminEditExamPageComponent },
  { path: 'admin/exams/report/:id', component: AdminExamReportPageComponent },
  { path: '**', redirectTo: '/home' }
];
