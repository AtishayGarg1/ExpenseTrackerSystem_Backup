import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { ExpenseService, Expense } from '../../expense.service';
import { CategoryService, Category } from '../../category.service';
import { AuthService } from '../../auth.service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-expenses',
  standalone: true,
  imports: [CommonModule, FormsModule, Sidebar, RouterLink],
  templateUrl: './expenses.html',
  styleUrl: './expenses.scss'
})
export class Expenses implements OnInit {
  expenses: Expense[] = [];
  filteredExpenses: Expense[] = [];
  categories: Category[] = [];
  user: any = null;
  isAdmin: boolean = false;
  showUserMenu: boolean = false;
  
  // Filter State
  filters = {
    searchQuery: '',
    categoryId: 0,
    paymentMode: '',
    startDate: '',
    endDate: ''
  };

  // Form State
  newExpense: Partial<Expense> = {
    categoryId: 0,
    amount: null as any,
    currency: 'INR',
    description: '',
    date: new Date(),
    paymentMode: 'Credit Card',
    tags: '',
    isRecurring: false
  };

  isSubmitting = false;
  isEditing = false;
  editingId: number | null = null;

  constructor(
    private expenseService: ExpenseService,
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
    this.loadExpenses();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getInitials(name: string): string {
    return this.authService.getInitials(name);
  }


  applyFilters() {
    this.filteredExpenses = this.expenses.filter(e => {
      const matchesSearch = !this.filters.searchQuery || 
        e.description.toLowerCase().includes(this.filters.searchQuery.toLowerCase()) ||
        (e.tags && e.tags.toLowerCase().includes(this.filters.searchQuery.toLowerCase()));
      
      const matchesCategory = Number(this.filters.categoryId) === 0 || e.categoryId == this.filters.categoryId;
      
      const matchesMode = !this.filters.paymentMode || e.paymentMode === this.filters.paymentMode;
      
      const eDate = new Date(e.date);
      const matchesStart = !this.filters.startDate || eDate >= new Date(this.filters.startDate);
      const matchesEnd = !this.filters.endDate || eDate <= new Date(this.filters.endDate);

      return matchesSearch && matchesCategory && matchesMode && matchesStart && matchesEnd;
    });
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories = data.filter(c => c.type === 'EXPENSE');
        if (this.categories.length > 0) this.newExpense.categoryId = this.categories[0].categoryId;
      },
      error: (err) => console.error('Failed to load categories', err)
    });
  }

  loadExpenses() {
    this.expenseService.getExpensesByUser().subscribe({
      next: (data) => {
        this.expenses = data;
        this.applyFilters();
      },
      error: (err) => console.error('Failed to load expenses', err)
    });
  }

  getCategoryColor(id: number): string {
    const cat = this.categories.find(c => c.categoryId === id);
    return cat ? cat.color : 'rgba(255,255,255,0.2)';
  }

  getCategoryIcon(id: number): string {
    const cat = this.categories.find(c => c.categoryId === id);
    return cat ? cat.icon : '💸';
  }

  onSubmit() {
    this.isSubmitting = true;
    
    if (!this.newExpense.date) {
      this.newExpense.date = new Date();
    }

    if (this.isEditing && this.editingId) {
      this.expenseService.updateExpense(this.editingId, this.newExpense).subscribe({
        next: () => {
          this.loadExpenses();
          this.resetForm();
        },
        error: (err) => {
          console.error('Failed to update expense', err);
          this.isSubmitting = false;
        }
      });
    } else {
      this.expenseService.addExpense(this.newExpense).subscribe({
        next: (savedExpense) => {
          this.expenses.unshift(savedExpense);
          this.applyFilters();
          this.resetForm();
        },
        error: (err) => {
          console.error('Failed to save expense', err);
          this.isSubmitting = false;
        }
      });
    }
  }

  onEdit(expense: Expense) {
    this.isEditing = true;
    this.editingId = expense.expenseId;
    this.newExpense = { ...expense, date: new Date(expense.date) };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDelete(id: number) {
    if (confirm('Are you sure you want to delete this expense?')) {
      this.expenseService.deleteExpense(id).subscribe({
        next: () => this.loadExpenses(),
        error: (err) => console.error('Failed to delete expense', err)
      });
    }
  }

  resetForm() {
    this.isSubmitting = false;
    this.isEditing = false;
    this.editingId = null;
    this.newExpense = {
      categoryId: this.categories.length > 0 ? this.categories[0].categoryId : 0,
      amount: null as any,
      currency: 'INR',
      description: '',
      date: new Date(),
      paymentMode: 'Credit Card',
      tags: '',
      isRecurring: false
    };
  }
}
