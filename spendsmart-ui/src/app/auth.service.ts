import { environment } from '../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl + '/users'; // Gateway URL

  constructor(private http: HttpClient) {}

  login(authRequest: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, authRequest).pipe(
      tap((response: any) => {
        if (response && response.token) {
          localStorage.setItem('jwt_token', response.token);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('jwt_token');
  }

  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/profile`);
  }

  register(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('jwt_token');
  }

  deleteAccount(password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/me/delete`, { password });
  }

  getInitials(fullName: string): string {
    if (!fullName) return 'U';
    const names = fullName.trim().split(/\s+/);
    if (names.length === 1) return names[0].charAt(0).toUpperCase();
    return (names[0].charAt(0) + names[names.length - 1].charAt(0)).toUpperCase();
  }

  isAdmin(): boolean {
    if (typeof window === 'undefined') return false;
    const token = localStorage.getItem('jwt_token');
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const role = payload.role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      return role === 'Admin';
    } catch {
      return false;
    }
  }
}



