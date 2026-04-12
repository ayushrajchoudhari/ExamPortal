import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-instruction-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './instruction-page.component.html',
  styleUrls: ['./instruction-page.component.scss'],
})
export class InstructionPage {
  agreed: boolean = false;

  handleStartExam() {
    if (this.agreed) {
      console.log('Starting exam...');
      // Navigate to exam (will be implemented when you add exam pages)
    }
  }
}
