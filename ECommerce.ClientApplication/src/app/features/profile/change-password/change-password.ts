import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/authService';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './change-password.html',
  styleUrls: ['./change-password.scss']
})
export class ChangePassword {
  
  passwordData = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  constructor(private authService: AuthService) {}

  onSubmit() {
    // 1. Basit Validasyonlar
    if (!this.passwordData.currentPassword || !this.passwordData.newPassword) {
      alert("Lütfen tüm alanları doldurunuz.");
      return;
    }

    if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
      alert("Yeni şifreler birbiriyle uyuşmuyor!");
      return;
    }

    if (this.passwordData.newPassword.length < 6) {
      alert("Yeni şifreniz en az 6 karakter olmalıdır.");
      return;
    }

    // 2. Servise İstek At
    this.authService.changePassword(this.passwordData).subscribe({
      next: (res) => {
        if (res.success) {
          alert("Şifreniz başarıyla güncellendi!");
          // Formu temizle
          this.passwordData = { currentPassword: '', newPassword: '', confirmPassword: '' };
        } else {
          alert(res.message || "İşlem başarısız.");
        }
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || "Şifre değiştirilemedi. Mevcut şifrenizi kontrol ediniz.");
      }
    });
  }
  
  onLogout() {
    this.authService.logout();
  }
}