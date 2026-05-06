import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Sidebar } from '../../shared/sidebar/sidebar';
import { IncomeService, Income } from '../../income.service';
import { AuthService } from '../../auth.service';
import { Router, RouterLink } from '@angular/router';
import { CategoryService, Category } from '../../category.service';

@Component({
  selector: 'app-income',
  standalone: true,
  imports: [CommonModule, FormsModule, Sidebar, RouterLink],
  templateUrl: './income.html',
  styleUrl: './income.scss'
})
export class IncomeComponent implements OnInit {
  incomes: Income[] = [];
  filteredIncomes: Income[] = [];
  categories: Category[] = [];
  
  filters = {
    searchQuery: '',
    startDate: '',
    endDate: ''
  };

  user: any = null;
  isAdmin: boolean = false;
  showUserMenu: boolean = false;

  newIncome: Partial<Income> = {
    source: 'Salary',
    amount: null as any,
    currency: 'INR',
    description: '',
    date: new Date(),
    isRecurring: false
  };

  isSubmitting = false;
  isEditing = false;
  editingId: number | null = null;

  constructor(
    private incomeService: IncomeService,
    private authService: AuthService,
    private categoryService: CategoryService,
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
    this.loadIncomes();
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe(res => {
      this.categories = res.filter(c => c.type === 'INCOME');
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getInitials(name: string): string {
    return this.authService.getInitials(name);
  }


  applyFilters() {
    this.filteredIncomes = this.incomes.filter(i => {
      const matchesSearch = !this.filters.searchQuery || 
        i.source.toLowerCase().includes(this.filters.searchQuery.toLowerCase()) ||
        (i.description && i.description.toLowerCase().includes(this.filters.searchQuery.toLowerCase()));
      
      const iDate = new Date(i.date);
      const matchesStart = !this.filters.startDate || iDate >= new Date(this.filters.startDate);
      const matchesEnd = !this.filters.endDate || iDate <= new Date(this.filters.endDate);

      return matchesSearch && matchesStart && matchesEnd;
    });
  }

  loadIncomes() {
    this.incomeService.getIncomesByUser().subscribe({
      next: (data) => {
        this.incomes = data;
        this.applyFilters();
      },
      error: (err) => console.error('Failed to load incomes', err)
    });
  }

  onSubmit() {
    this.isSubmitting = true;
    
    if (!this.newIncome.date) {
      this.newIncome.date = new Date();
    }

    if (this.isEditing && this.editingId) {
      this.incomeService.updateIncome(this.editingId, this.newIncome).subscribe({
        next: () => {
          this.loadIncomes();
          this.resetForm();
        },
        error: (err) => {
          console.error('Failed to update income', err);
          this.isSubmitting = false;
        }
      });
    } else {
      this.incomeService.addIncome(this.newIncome).subscribe({
        next: (savedIncome) => {
          this.incomes.unshift(savedIncome);
          this.resetForm();
        },
        error: (err) => {
          console.error('Failed to save income', err);
          this.isSubmitting = false;
        }
      });
    }
  }

  onEdit(income: Income) {
    this.isEditing = true;
    this.editingId = income.incomeId;
    this.newIncome = { ...income, date: new Date(income.date) };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDelete(id: number) {
    if (confirm('Are you sure you want to delete this income record?')) {
      this.incomeService.deleteIncome(id).subscribe({
        next: () => this.loadIncomes(),
        error: (err) => console.error('Failed to delete income', err)
      });
    }
  }

  resetForm() {
    this.isSubmitting = false;
    this.isEditing = false;
    this.editingId = null;
    this.newIncome = {
      source: this.categories.length > 0 ? this.categories[0].name : 'Salary',
      amount: null as any,
      currency: 'INR',
      description: '',
      date: new Date(),
      isRecurring: false
    };
  }
}
