import { Routes } from '@angular/router';
import { InstructionPage } from './components/instruction-page/instruction-page.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { ExamPageComponent } from './components/exam-page/exam-page.component';
import { ResultPageComponent } from './components/result-page/result-page.component';
import { AdminHomeComponent } from './components/admin/admin-home-page/admin-home-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'instructions', component: InstructionPage },
  { path: 'login', component: LoginPageComponent },
  { path: 'home', component: HomePageComponent },
  { path: 'exam', component: ExamPageComponent },
  { path: 'user/instructions/:id', component: InstructionPage },
  { path: 'examination/:attemptId', component: ExamPageComponent },
  { path: 'result/:attemptId', component: ResultPageComponent },
  { path: 'admin/home', component: AdminHomeComponent },
  { path: '**', redirectTo: '/home' }
];
