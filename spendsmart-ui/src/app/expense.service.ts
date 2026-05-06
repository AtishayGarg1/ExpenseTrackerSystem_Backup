import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Expense {
  expenseId: number;
  userId: number;
  categoryId: number;
  amount: number;
  currency: string;
  description: string;
  date: Date;
  paymentMode: string;
  tags: string;
  isRecurring: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private apiUrl = environment.apiUrl + '/';

  constructor(private http: HttpClient) {}

  getTotalExpenseForMonth(month: number, year: number): Observable<{ total: number }> {
    return this.http.get<{ total: number }>(`${this.apiUrl}/total?month=${month}&year=${year}`);
  }

  getExpensesByUser(): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.apiUrl}/user`);
  }

  addExpense(request: Partial<Expense>): Observable<Expense> {
    return this.http.post<Expense>(this.apiUrl, request);
  }

  updateExpense(id: number, request: Partial<Expense>): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  deleteExpense(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}

