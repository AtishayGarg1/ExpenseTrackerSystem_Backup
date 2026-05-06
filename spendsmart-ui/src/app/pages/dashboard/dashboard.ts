import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { IncomeService } from '../../income.service';
import { ExpenseService } from '../../expense.service';
import { BudgetService, Budget } from '../../budget.service';
import { CategoryService, Category } from '../../category.service';
import { AuthService } from '../../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, Sidebar, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard implements OnInit {
  totalIncome: number = 0;
  totalExpense: number = 0;
  savingsRate: number = 0;
  categories: Category[] = [];
  expenses: any[] = [];
  budgets: Budget[] = [];
  showUserMenu: boolean = false;
  user: any = null;
  isAdmin: boolean = false;

  constructor(
      private incomeService: IncomeService,
      private expenseService: ExpenseService,
      private budgetService: BudgetService,
      private categoryService: CategoryService,
      private authService: AuthService,
      private router: Router
  ) {}

  ngOnInit() {
      // Fetch authenticated user profile
      this.authService.getProfile().subscribe({
        next: (profile) => {
          this.user = profile;
          const userRole = profile.role || profile.Role || profile['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
          this.isAdmin = userRole.toString().toLowerCase() === 'admin' || profile.email === 'admin@spendsmart.com' || profile.Email === 'admin@spendsmart.com';
        },
        error: () => {
          console.log('Failed to load profile');
          this.isAdmin = false;
        }
      });

      const now = new Date();
      const incomeObs = this.incomeService.getTotalIncomeForMonth(now.getMonth() + 1, now.getFullYear());
      const expenseObs = this.expenseService.getTotalExpenseForMonth(now.getMonth() + 1, now.getFullYear());

      forkJoin([incomeObs, expenseObs]).subscribe({
        next: ([incomeData, expenseData]) => {
          this.totalIncome = incomeData.total;
          this.totalExpense = expenseData.total;
          
          if (this.totalIncome > 0) {
            this.savingsRate = Math.round(((this.totalIncome - this.totalExpense) / this.totalIncome) * 100);
          } else {
            this.savingsRate = 0;
          }
        },
        error: (err) => console.error('Failed to load financial stats', err)
      });

      this.expenseService.getExpensesByUser().subscribe({
          next: (data) => this.expenses = data,
          error: (err) => console.error('Failed to load recent transactions')
      });

      this.categoryService.getCategories().subscribe({
        next: (data) => this.categories = data,
        error: (err) => console.error('Failed to load categories', err)
      });

      this.budgetService.getActiveBudgets().subscribe({
          next: (data) => this.budgets = data, 
          error: (err) => console.error('Failed to load dashboard budgets')
      });
  }

  getCategoryName(id?: number): string {
    if (!id) return 'Global/All';
    const cat = this.categories.find(c => c.categoryId === id);
    return cat ? cat.name : 'All Spending';
  }

  getPercentage(spent: number, limit: number): number {
    if (limit === 0) return 0;
    const p = (spent / limit) * 100;
    return p > 100 ? 100 : p;
  }

  getCategoryColor(id: number): string {
    const cat = this.categories.find(c => c.categoryId === id);
    return cat ? cat.color : 'rgba(255,255,255,0.2)';
  }

  getCategoryIcon(id: number): string {
    const cat = this.categories.find(c => c.categoryId === id);
    return cat ? cat.icon : '💸';
  }

  getBarColor(spent: number, limit: number): string {
    const p = this.getPercentage(spent, limit);
    if (p >= 100) return 'var(--danger-color)';
    if (p >= 80) return 'var(--warning-color)';
    return 'var(--success-color)';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getInitials(name: string): string {
    return this.authService.getInitials(name);
  }

  getSavingsStatus(): { label: string, color: string } {
    if (this.savingsRate >= 50) return { label: 'Excellent', color: 'var(--success-rgb)' };
    if (this.savingsRate >= 20) return { label: 'Good', color: 'var(--accent-rgb)' };
    if (this.savingsRate > 0) return { label: 'Low', color: 'var(--warning-rgb)' };
    if (this.savingsRate === 0) return { label: 'Neutral', color: 'var(--text-muted)' };
    return { label: 'Deficit', color: 'var(--danger-rgb)' };
  }
}
