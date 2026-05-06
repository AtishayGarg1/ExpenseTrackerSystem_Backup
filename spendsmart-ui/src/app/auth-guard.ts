import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('jwt_token');
    if (token) {
      // If the route has a data.role requirement, check it
      const requiredRole = route.data?.['role'];
      if (requiredRole) {
        try {
          // Manually decode JWT payload without extra library
          const payload = JSON.parse(atob(token.split('.')[1]));
          const userRole = payload.role || payload.Role || payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
          const userEmail = payload.email || payload.Email || payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || payload.unique_name;
          
          const isAdmin = userRole === 'Admin' || userRole === 'admin' || userEmail === 'admin@spendsmart.com';

          if (requiredRole) {
            if (userRole !== requiredRole && !isAdmin) {
              router.navigate(['/dashboard']);
              return false;
            }
          } else {
            // If the route doesn't specifically require a role, but the user is an Admin,
            // force them into the admin panel (prevent access to normal user features).
            if (isAdmin) {
              router.navigate(['/admin']);
              return false;
            }
          }
        } catch (e) {
          router.navigate(['/login']);
          return false;
        }
      }
      return true;
    }
  }
  
  router.navigate(['/login']);
  return false;
};
