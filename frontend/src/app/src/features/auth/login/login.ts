import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  isLoading = false;
  errorMessage: string | null = null;

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) return;

    this.isLoading = true;
    this.errorMessage = null;

    const { email, password } = this.form.value;

    this.authService.login(email!, password!).subscribe({
      next: (res) => {
        this.authService.saveToken(res.token);
        this.router.navigate(['/tasks']);
      },
      error: (err: HttpErrorResponse) => {
        this.isLoading = false;
        if (err.status === 401) {
          this.errorMessage = 'Invalid email or password.';
        } else {
          this.errorMessage = 'An unexpected error occurred. Please try again.';
        }
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
}
