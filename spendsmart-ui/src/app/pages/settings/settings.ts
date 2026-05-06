import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../auth.service';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { CategoryService, Category } from '../../category.service';
import { BudgetService } from '../../budget.service';
import { environment } from '../../../../environments/environment';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../auth.service';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { CategoryService, Category } from '../../category.service';
import { BudgetService } from '../../budget.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './settings.html',
  styleUrl: './settings.css'
})
export class SettingsComponent implements OnInit {
  user: any = { fullName: '', email: '', currency: 'INR' };
  passwordData = { oldPassword: '', newPassword: '', confirmPassword: '' };
  deleteAccountPassword: string = '';

  categories: Category[] = [];
  categoryForm = { name: '', icon: '📂', color: '#6366f1', type: 'EXPENSE' };

  emojiList: string[] = [
    '💰', '🏦', '💳', '💸', '🧾', '📈', '🏠', '🛒',
    '🍔', '🚗', '🎮', '🎬', '👕', '🏥', '🎓', '💼',
    '🎁', '✈️', '📱', '💡', '🧼', '🐶', '🏋️', '🧘',
    '🌳', '🚲', '🍿', '🎸', '💻', '🔌', '📦'
  ];

  isSaving: boolean = false;
  activeTab: 'profile' | 'preferences' | 'security' | 'categories' | 'danger' = 'profile';
  isAdmin: boolean = false;
  showUserMenu: boolean = false;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private categoryService: CategoryService,
    private budgetService: BudgetService,
    private router: Router
  ) { }

  ngOnInit() {
    this.authService.getProfile().subscribe({
      next: (profile) => {
        this.user = profile;
        const userRole = profile.role || profile['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
        this.isAdmin = userRole === 'Admin';
      },
      error: () => this.isAdmin = false
    });
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe(res => {
      this.categories = res;
    });
  }

  saveCategory() {
    if (!this.categoryForm.name) return;
    this.isSaving = true;
    
    const autoColor = this.categoryForm.type === 'INCOME' ? '#22c55e' : '#ef4444';

    this.categoryService.addCategory(
      this.categoryForm.name,
      autoColor,
      this.categoryForm.icon,
      this.categoryForm.type
    ).subscribe({
      next: (newCat: Category) => {
        this.isSaving = false;
        this.categoryForm.name = '';
        this.loadCategories();
      },
      error: () => {
        this.isSaving = false;
        alert('Failed to add category');
      }
    });
  }

  deleteCategory(id: number) {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => this.loadCategories(),
        error: () => alert('Failed to delete category. It might be in use.')
      });
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }


  loadProfile() {
    this.http.get(`${environment.apiUrl}/users/profile`).subscribe((res: any) => {
      this.user = res;
    });
  }

  updateProfile() {
    this.isSaving = true;
    this.http.put(`${environment.apiUrl}/users/profile`, {
      fullName: this.user.fullName
    }).subscribe({
      next: () => {
        this.isSaving = false;
        alert('Profile updated successfully!');
      },
      error: () => {
        this.isSaving = false;
        alert('Failed to update profile.');
      }
    });
  }

  updateCurrency() {
    this.http.put(`${environment.apiUrl}/users/currency`, {
      currency: this.user.currency
    }).subscribe(() => alert('Currency updated!'));
  }

  changePassword() {
    if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
      alert('Passwords do not match');
      return;
    }
    this.http.put(`${environment.apiUrl}/users/password`, this.passwordData)
      .subscribe({
        next: () => alert('Password changed!'),
        error: () => alert('Failed to change password.')
      });
  }

  onDeleteAccount() {
    if (!this.deleteAccountPassword) {
      alert('Please enter your password to confirm account deletion.');
      return;
    }

    if (confirm('CRITICAL WARNING: Are you sure you want to PERMANENTLY delete your account? All your data (expenses, income, budgets) will be lost forever. This action cannot be undone.')) {
      this.authService.deleteAccount(this.deleteAccountPassword).subscribe({
        next: () => {
          alert('Your account has been successfully deleted. We are sorry to see you go.');
          this.logout();
        },
        error: (err) => {
          const msg = err.error?.message || 'Failed to delete account. Please check your password.';
          alert(msg);
        }
      });
    }
  }

  getInitials(name: string): string {
    return this.authService.getInitials(name);
  }
}
