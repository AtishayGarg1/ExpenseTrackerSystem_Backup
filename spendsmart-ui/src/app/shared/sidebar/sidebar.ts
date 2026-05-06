import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { IncomeService } from '../../income.service';
import { AuthService } from '../../auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar implements OnInit {
  netBalance: number = 0;
  isAdmin: boolean = false;

  constructor(
    private incomeService: IncomeService,
    private authService: AuthService
  ) {}

  ngOnInit() {
      this.authService.getProfile().subscribe({
        next: (profile) => {
          const role = profile.role || profile.Role || profile['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || '';
          this.isAdmin = role.toString().toLowerCase() === 'admin' || profile.email === 'admin@spendsmart.com' || profile.Email === 'admin@spendsmart.com';
        },
        error: () => this.isAdmin = false
      });

      this.incomeService.getNetBalance().subscribe({
        next: (data) => this.netBalance = data.netBalance,
        error: (err) => console.warn('Could not fetch net balance')
      });
  }
}
