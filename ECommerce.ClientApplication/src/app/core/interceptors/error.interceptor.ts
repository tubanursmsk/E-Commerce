import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error) => {
      if (error) {
        // 404: API bulunamadı
        if (error.status === 404) {
          router.navigate(['/notfound']);
        }
        
        // 500: Sunucu Hatası
        // 0: Sunucuya Hiç Ulaşılamıyor (Backend Kapalı)
        if (error.status === 500 || error.status === 0) {
          router.navigate(['/servererror']);
        }
      }
      return throwError(() => error);
    })
  );
};