import { environment } from '../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Budget {
  budgetId: number;
  userId: number;
  categoryId?: number;
  name: string;
  limitAmount: number;
  spentAmount: number;
  currency: string;
  period: string;
  startDate: Date;
  endDate: Date;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private apiUrl = environment.apiUrl + '/budgets';

  constructor(private http: HttpClient) {}

  getActiveBudgets(): Observable<Budget[]> {
    return this.http.get<Budget[]>(`${this.apiUrl}/active`);
  }

  addBudget(request: Partial<Budget>): Observable<Budget> {
    return this.http.post<Budget>(this.apiUrl, request);
  }

  updateBudget(id: number, request: Partial<Budget>): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  deleteBudget(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}



