import { Routes } from '@angular/router';
import { InstructionPage } from './components/instruction-page/instruction-page';

export const routes: Routes = [
  { path: '', redirectTo: '/instructions', pathMatch: 'full' },
  { path: 'instructions', component: InstructionPage }
];
