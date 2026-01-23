import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/authService';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html'
})
export class Register {
  registerObj = {
    firstName: '',
    lastName: '',
    email: '',
    password: ''
  };

  constructor(private authService: AuthService, private router: Router) {}

  onRegister() {
    if (this.registerObj.firstName && this.registerObj.email && this.registerObj.password) {
      this.authService.register(this.registerObj).subscribe({
        next: (res) => {
          if (res.success) {
            alert("Kayıt başarılı! Giriş yapabilirsiniz.");
            this.router.navigate(['/login']);
          } else {
            alert(res.message);
          }
        },
        error: (err) => {
          console.error("Kayıt hatası:", err);
          alert("Bir hata oluştu.");
        }
      });
    }
  }
}