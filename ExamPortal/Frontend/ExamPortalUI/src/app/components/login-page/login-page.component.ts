import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, LoginRequestDto } from '../../core/services/auth/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ FormsModule ],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
})
export class LoginPageComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  // UI State Signals
  public isLoginMode = signal<boolean>(true);
  public isLoading = signal<boolean>(false);
  public errorMessage = signal<string | null>(null);

  // Form Data Signal
  public formData = signal({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: ''
  });

  public toggleMode(): void {
    this.isLoginMode.update(mode =>!mode);
    this.errorMessage.set(null);
  }

  public onSubmit(): void {
    this.errorMessage.set(null);
    const data = this.formData();

    if (!this.isLoginMode() && data.password!== data.confirmPassword) {
      this.errorMessage.set("Passwords do not match.");
      return;
    }

    this.isLoading.set(true);

    if (this.isLoginMode()) {
      // Execute Login
      const credentials: LoginRequestDto = { email: data.email, password: data.password };
      
      this.authService.login(credentials).subscribe({
        next: (response) => {
          this.isLoading.set(false);
          // Route dynamically based on role claims
          if (response.role === 'Admin') {
            this.router.navigate(['/admin/home']);
          } else {
            this.router.navigate(['/user/home']);
          }
        },
        error: (err) => {
          this.isLoading.set(false);
          this.errorMessage.set(err.error?.message || 'Invalid email or password');
        }
      });
    } else {
      // Execute SignUp (Mocked until backend endpoint is built)
      setTimeout(() => {
        this.isLoading.set(false);
        this.errorMessage.set("Signup endpoint not yet wired in backend. Please log in.");
        this.isLoginMode.set(true);
      }, 1000);
    }
  }
}