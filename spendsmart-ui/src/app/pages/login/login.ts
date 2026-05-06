import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../auth.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  isLoginMode = true;
  
  // Form fields
  email = '';
  password = '';
  confirmPassword = '';
  fullName = '';
  preferredCurrency = 'INR';

  errorMessage = '';
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {}

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    this.errorMessage = '';
  }

  onSubmit() {
    this.isLoading = true;
    this.errorMessage = '';
    
    if (this.isLoginMode) {
      this.authService.login({ email: this.email, password: this.password }).subscribe({
        next: () => {
          this.isLoading = false;
          if (this.authService.isAdmin()) {
            this.router.navigate(['/admin']);
          } else {
            this.router.navigate(['/dashboard']);
          }
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Invalid email or password.';
          this.isLoading = false;
        }
      });
    } else {
      if (this.password !== this.confirmPassword) {
        this.errorMessage = 'Passwords do not match.';
        this.isLoading = false;
        return;
      }
      const payload = {
        email: this.email,
        password: this.password,
        fullName: this.fullName,
        currency: this.preferredCurrency
      };
      // We will define register in AuthService next
      this.authService.register(payload).subscribe({
        next: () => {
          this.isLoading = false;
          alert('Registration successful! Please login.');
          this.isLoginMode = true; // Switch back to login
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Registration failed.';
          this.isLoading = false;
        }
      });
    }
  }
}
