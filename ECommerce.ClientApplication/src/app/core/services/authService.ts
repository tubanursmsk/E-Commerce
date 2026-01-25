import { CartService } from './cartService';
import { Injectable, signal } from '@angular/core';
import { BaseService } from './baseService'; // Senin BaseService'in
import { Router } from '@angular/router';
import { ApiResponse } from '../models/apiResponse'; // ApiResponse modelini import et
import { tap } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Kullanıcı bilgisini tutan sinyal
  currentUser = signal<{ name: string, email: string, role: string } | null>(null);

  constructor(private baseService: BaseService, private router: Router, private cartService: CartService) {
    // Sayfa yenilendiğinde Token varsa kullanıcıyı geri yükle
    const token = localStorage.getItem('token');
    if (token) {
      this.setUserFromToken(token);
    }
  }

  // LOGIN: Gerçek API Bağlantısı
  login(credentials: any) {
    return this.baseService.post<ApiResponse<string>>('Auth/Login', credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          const token = response.data;

          // 1. Token'ı sakla
          localStorage.setItem('token', token);

          // 2. Token'ı çöz ve kullanıcıyı sinyale ata
          this.setUserFromToken(token);
        }
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUser.set(null);
    
    // ÇIKIŞ YAPILDIĞINDA SEPETİ TEMİZLE
    this.cartService.clearCart(); 
    
    this.router.navigate(['/']);
  }

  isLoggedIn(): boolean {
    return !!this.currentUser();
  }

  // YARDIMCI METOD: JWT Token'ı çözüp içindeki bilgileri okur
  private setUserFromToken(token: string) {
    try {
      // JWT 3 kısımdan oluşur: Header.Payload.Signature
      // Bizim işimiz Payload (ortadaki) kısmıyla
      const payloadBase64 = token.split('.')[1];
      const payloadJson = atob(payloadBase64);
      const payload = JSON.parse(payloadJson);

      // Backend'de claim isimleri: 
      // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" -> Email
      // "fullName" -> Ad Soyad (Bizim eklediğimiz)
      // "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" -> Role

      const user = {
        name: payload['fullName'] || 'Kullanıcı', // fullName claim'ini oku
        email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      };

      this.currentUser.set(user);

    } catch (error) {
      console.error('Token çözülemedi:', error);
      this.logout();
    }
  }

  register(user: any) {
    // Backend'de yeni açtığımız endpoint'e gidiyor
    return this.baseService.post<ApiResponse<string>>('Auth/RegisterCustomer', user);
  }

  updateProfile(userDto: any) {
    return this.baseService.post<ApiResponse<boolean>>('Auth/UpdateProfile', userDto);
  }
}