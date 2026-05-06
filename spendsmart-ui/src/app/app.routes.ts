import { Routes } from '@angular/router';
import { authGuard } from './auth-guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./pages/landing/landing').then(m => m.Landing) },
  { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.Login) },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./pages/dashboard/dashboard').then(m => m.Dashboard),
    canActivate: [authGuard]
  },
  { 
    path: 'expenses', 
    loadComponent: () => import('./pages/expenses/expenses').then(m => m.Expenses),
    canActivate: [authGuard]
  },
  { 
    path: 'income', 
    loadComponent: () => import('./pages/income/income').then(m => m.IncomeComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'budgets', 
    loadComponent: () => import('./pages/budgets/budgets').then(m => m.BudgetsComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'settings', 
    loadComponent: () => import('./pages/settings/settings').then(m => m.SettingsComponent),
    canActivate: [authGuard]
  },
  { 
    path: 'admin', 
    loadComponent: () => import('./pages/admin/admin').then(m => m.AdminComponent),
    canActivate: [authGuard],
    data: { role: 'Admin' }
  },
  { path: '**', redirectTo: '' }
];
