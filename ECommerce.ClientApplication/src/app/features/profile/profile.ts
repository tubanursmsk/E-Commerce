import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/authService';
import { CustomerService } from '../../core/services/customerService'; // Ekle

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.scss']
})
export class Profile implements OnInit {

  // DTO ile uyumlu model
  user = {
    firstName: '',
    lastName: '',
    email: '',
    phone: '', // Backend DTO'da Phone
    address: '',
    city: '',
    birthDate: '', // Bu Backend DTO'da yoksa eklenmeli veya UI'da pasif kalmalı
    gender: '',    // Bu Backend DTO'da yoksa eklenmeli
    marketingConsent: false
  };

  constructor(
    public authService: AuthService,
    private customerService: CustomerService // Ekle
  ) { }

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile() {
    this.customerService.getProfile().subscribe({
      next: (data) => {
        if (data) {
          // Backend'den gelen verileri forma eşle
          this.user.firstName = data.firstName;
          this.user.lastName = data.lastName;
          this.user.email = data.email;
          this.user.phone = data.phone || '';
          this.user.address = data.address || '';
          this.user.city = data.city || '';
        }
      },
      error: (err) => console.error("Profil yüklenemedi", err)
    });
  }

  onSave() {
    // Backend'in beklediği DTO formatı
    const updateDto = {
      firstName: this.user.firstName,
      lastName: this.user.lastName,
      email: this.user.email,
      phone: this.user.phone || '',
      address: this.user.address || '',
      city: this.user.city || 'Belirtilmemiş', // Şehir boş gitmesin
      // UserId'yi göndermiyoruz, Backend token'dan alacak
    };
    console.log("Gönderilen DTO:", updateDto); // Konsoldan kontrol etmek için yazıldı

    this.customerService.updateProfile(updateDto).subscribe({
      next: (res) => {
        if (res.success) 
          {
            alert("Bilgileriniz başarıyla güncellendi!");
          } else {
          // Eğer backend success:false dönüyorsa mesajı göster
          alert(res?.message || "İşlem başarısız.");
        }
      },
      error: (err) => {
        console.error(err);
        // Hatanın detayını görmek için:
        if (err.error && err.error.errors) {
          // Backend validasyon hatalarını (Hangi alan eksik?) alert ile göster
          const validationErrors = JSON.stringify(err.error.errors);
          alert("Hata: " + validationErrors);
        } else {
          alert("Güncelleme sırasında hata oluştu.");
        }
      }
    });
  }
  

  onLogout() {
    this.authService.logout();
  }
}