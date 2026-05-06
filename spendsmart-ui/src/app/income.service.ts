import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Income {
  incomeId: number;
  userId: number;
  source: string;
  amount: number;
  currency: string;
  description: string;
  date: Date;
  isRecurring: boolean;
  recurrenceType?: string;
}

@Injectable({
  providedIn: 'root'
})
export class IncomeService {
  private apiUrl = environment.apiUrl + '/';

  constructor(private http: HttpClient) {}

  getNetBalance(): Observable<{ netBalance: number }> {
    return this.http.get<{ netBalance: number }>(`${this.apiUrl}/net-balance`);
  }

  getTotalIncomeForMonth(month: number, year: number): Observable<{ total: number }> {
    return this.http.get<{ total: number }>(`${this.apiUrl}/total?month=${month}&year=${year}`);
  }

  getIncomesByUser(): Observable<Income[]> {
    return this.http.get<Income[]>(`${this.apiUrl}/user`);
  }

  addIncome(request: Partial<Income>): Observable<Income> {
    return this.http.post<Income>(this.apiUrl, request);
  }

  updateIncome(id: number, request: Partial<Income>): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  deleteIncome(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}

