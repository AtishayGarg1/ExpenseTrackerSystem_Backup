import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../auth.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class AdminComponent implements OnInit {
  activeTab: string = 'dashboard';
  analytics: any = { totalUsers: 0, totalExpenses: 0, totalIncome: 0, platformStatus: 'Healthy' };
  users: any[] = [];
  expenses: any[] = [];
  incomes: any[] = [];
  auditLogs: any[] = [];
  isLoading: boolean = false;
  passwordData = { oldPassword: '', newPassword: '', confirmPassword: '' };
  adminProfile = { fullName: '' };
  isSaving: boolean = false;


  private readonly API_BASE = environment.apiUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.getProfile().subscribe(profile => {
      this.adminProfile.fullName = profile.fullName || profile.FullName || 'Platform Admin';
    });
    this.switchTab('dashboard');
  }

  switchTab(tab: string) {
    this.activeTab = tab;
    this.loadTabData();
  }

  loadTabData() {
    this.isLoading = true;
    switch(this.activeTab) {
      case 'dashboard':
        this.http.get(`${this.API_BASE}/reports/admin/analytics`).subscribe((res: any) => {
          this.analytics = res;
          this.isLoading = false;
        });
        break;
      case 'users':
        this.http.get(`${this.API_BASE}/users/all`).subscribe((res: any) => {
          this.users = res;
          this.isLoading = false;
        });
        break;
      case 'expenses':
        this.http.get(`${this.API_BASE}/reports/admin/expenses`).subscribe((res: any) => {
          this.expenses = res;
          this.isLoading = false;
        });
        break;
      case 'incomes':
        this.http.get(`${this.API_BASE}/reports/admin/incomes`).subscribe((res: any) => {
          this.incomes = res;
          this.isLoading = false;
        });
        break;
      case 'logs':
        this.http.get(`${this.API_BASE}/reports/admin/audit-logs`).subscribe((res: any) => {
          this.auditLogs = res;
          this.isLoading = false;
        });
        break;
    }
  }

  suspendUser(userId: number) {
    if (confirm('Suspend this user account?')) {
      this.http.put(`${this.API_BASE}/users/${userId}/suspend`, {}).subscribe(() => this.switchTab('users'));
    }
  }

  unsuspendUser(userId: number) {
    if (confirm('Restore this user account?')) {
      this.http.put(`${this.API_BASE}/users/${userId}/unsuspend`, {}).subscribe(() => this.switchTab('users'));
    }
  }

  deleteUser(userId: number) {
    if (confirm('PERMANENTLY DELETE this user?')) {
      this.http.delete(`${this.API_BASE}/users/${userId}`).subscribe(() => this.switchTab('users'));
    }
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  changePassword() {
    if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
      alert('Passwords do not match');
      return;
    }
    this.isSaving = true;
    this.http.put(`${this.API_BASE}/users/password`, this.passwordData)
      .subscribe({
        next: () => {
          this.isSaving = false;
          alert('Admin password changed successfully!');
          this.passwordData = { oldPassword: '', newPassword: '', confirmPassword: '' };
        },
        error: () => {
          this.isSaving = false;
          alert('Failed to change password. Please check your current password.');
        }
      });
  }

  updateAdminProfile() {
    if (!this.adminProfile.fullName) return;
    this.isSaving = true;
    this.http.put(`${this.API_BASE}/users/profile`, { fullName: this.adminProfile.fullName })
      .subscribe({
        next: () => {
          this.isSaving = false;
          alert('Admin profile updated successfully!');
        },
        error: () => {
          this.isSaving = false;
          alert('Failed to update admin profile.');
        }
      });
  }
}
