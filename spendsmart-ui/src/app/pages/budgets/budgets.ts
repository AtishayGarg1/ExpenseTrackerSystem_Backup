import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { BudgetService, Budget } from '../../budget.service';
import { CategoryService, Category } from '../../category.service';
import { AuthService } from '../../auth.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-budgets',
  standalone: true,
  imports: [CommonModule, FormsModule, Sidebar, RouterLink],
  templateUrl: './budgets.html',
  styleUrl: './budgets.scss'
})
export class BudgetsComponent implements OnInit {
  budgets: Budget[] = [];
  categories: Category[] = [];
  user: any = null;
  isAdmin: boolean = false;
  showUserMenu: boolean = false;
  
  newBudget: Partial<Budget> = {
    categoryId: undefined,
    name: '',
    limitAmount: null as any,
    currency: 'INR',
    period: 'Monthly',
    startDate: new Date(),
    endDate: new Date(new Date().setMonth(new Date().getMonth() + 1))
  };

  isSubmitting = false;
  isEditing = false;
  editingId: number | null = null;

  constructor(
    private budgetService: BudgetService,
    private categoryService: CategoryService,
    private authService: AuthService,
    private router: Router
  ) {}

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
    this.loadBudgets();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getInitials(name: string): string {
    return this.authService.getInitials(name);
  }


  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (data) => this.categories = data.filter(c => c.type === 'EXPENSE'),
      error: (err) => console.error('Failed to load categories', err)
    });
  }

  loadBudgets() {
    this.budgetService.getActiveBudgets().subscribe({
      next: (data) => this.budgets = data,
      error: (err) => console.error('Failed to load budgets', err)
    });
  }

  getCategoryName(id?: number): string {
    if (!id) return 'Global/All';
    const cat = this.categories.find(c => c.categoryId === id);
    return cat ? cat.name : 'Unknown';
  }

  getCategoryColor(id?: number): string {
    if (!id) return 'rgba(59, 130, 246, 0.4)';
    const cat = this.categories.find(c => c.categoryId === id);
    return cat ? cat.color : 'rgba(255,255,255,0.2)';
  }

  getPercentage(spent: number, limit: number): number {
    if (limit === 0) return 0;
    const p = (spent / limit) * 100;
    return p > 100 ? 100 : p;
  }

  getBarColor(spent: number, limit: number): string {
    const p = this.getPercentage(spent, limit);
    if (p >= 100) return 'var(--danger-color)';
    if (p >= 80) return 'var(--warning-color)';
    return 'var(--success-color)';
  }

  onSubmit() {
    this.isSubmitting = true;
    
    if (!this.newBudget.name) {
       this.newBudget.name = this.getCategoryName(this.newBudget.categoryId) + ' Budget';
    }

    if (!this.newBudget.startDate) this.newBudget.startDate = new Date();
    if (!this.newBudget.endDate) this.newBudget.endDate = new Date(new Date().setMonth(new Date().getMonth() + 1));

    if (this.newBudget.categoryId?.toString() === "null") {
        this.newBudget.categoryId = undefined;
    }

    if (this.isEditing && this.editingId) {
      this.budgetService.updateBudget(this.editingId, this.newBudget).subscribe({
        next: () => {
          this.loadBudgets();
          this.resetForm();
        },
        error: (err) => {
          console.error('Failed to update budget', err);
          this.isSubmitting = false;
        }
      });
    } else {
      this.budgetService.addBudget(this.newBudget).subscribe({
        next: (savedBudget) => {
          this.budgets.push(savedBudget);
          this.resetForm();
        },
        error: (err) => {
          console.error('Failed to create budget', err);
          this.isSubmitting = false;
        }
      });
    }
  }

  onEdit(budget: Budget) {
    this.isEditing = true;
    this.editingId = budget.budgetId;
    this.newBudget = { 
      ...budget, 
      startDate: new Date(budget.startDate),
      endDate: new Date(budget.endDate)
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDelete(id: number) {
    if (confirm('Are you sure you want to delete this budget enforcement?')) {
      this.budgetService.deleteBudget(id).subscribe({
        next: () => this.loadBudgets(),
        error: (err) => console.error('Failed to delete budget', err)
      });
    }
  }

  resetForm() {
    this.isSubmitting = false;
    this.isEditing = false;
    this.editingId = null;
    this.newBudget = {
      categoryId: undefined,
      name: '',
      limitAmount: null as any,
      currency: 'INR',
      period: 'Monthly',
      startDate: new Date(),
      endDate: new Date(new Date().setMonth(new Date().getMonth() + 1))
    };
  }
}
