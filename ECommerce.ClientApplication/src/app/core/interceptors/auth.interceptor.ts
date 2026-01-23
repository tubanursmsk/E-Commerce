import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // 1. Tarayıcı hafızasından token'ı al
  const token = localStorage.getItem('token');

  // 2. Eğer token varsa, isteğin kopyasını al ve header'a ekle
  if (token) {
    const clonedRequest = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    // Token eklenmiş isteği gönder
    return next(clonedRequest);
  }

  // Token yoksa isteği olduğu gibi gönder (Login/Register gibi)
  return next(req);
};