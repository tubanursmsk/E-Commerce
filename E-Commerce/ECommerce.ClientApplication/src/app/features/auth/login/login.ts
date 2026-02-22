import { Component, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/authService';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html',
  changeDetection: ChangeDetectionStrategy.Default
})
export class LoginComponent {
  email = '';
  password = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  onLogin() {
    if (this.email && this.password) {
      // isLoading = true; // İstersen loading ekleyebilirsin

      this.authService.login({ email: this.email, password: this.password })
        .subscribe({
          next: (res) => {
            if (res.success) {
              this.router.navigate(['/']); // Başarılıysa yönlendir
            } else {
              alert(res.message); // Hata mesajı (Örn: Şifre yanlış)
            }
          },
          error: (err) => {
            console.error("Giriş hatası:", err);
            alert("Giriş yapılamadı. Lütfen bilgilerinizi kontrol edin.");
          }
        });
    } 
    this.cdr.detectChanges();
  }
}