import { Routes } from '@angular/router';
import { InstructionPage } from './components/instruction-page/instruction-page.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { ExamPageComponent } from './components/exam-page/exam-page.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'instructions', component: InstructionPage },
  { path: 'login', component: LoginPageComponent },
  { path: 'home', component: HomePageComponent },
  { path: 'exam', component: ExamPageComponent },
  { path: '**', redirectTo: '/home' }
];
