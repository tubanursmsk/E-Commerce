import { Component, OnInit, AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CartService } from '../../core/services/cartService';
import { OrderService } from '../../core/services/orderService';
import { CustomerService } from '../../core/services/customerService';
import { AuthService } from '../../core/services/authService';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './checkout.html',
  styleUrls: ['./checkout.scss'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class CheckoutComponent implements OnInit {
  
  // Adres Formu
  address = {
    title: 'Ev Adresim',
    city: '',
    fullAddress: ''
  };

  // Kredi Kartı Formu (Dummy)
  card = {
    holderName: '',
    number: '',
    expiry: '',
    cvv: ''
  };

  isLoading = false;
  customerId: string | null = null;

  constructor(
    private cdr: ChangeDetectorRef,
    public cartService: CartService,
    private orderService: OrderService,
    private customerService: CustomerService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // 1. Sepet boşsa ana sayfaya at
    if (this.cartService.totalItems() === 0) {
      this.router.navigate(['/']);
      return;
    }

    // 2. Müşteri bilgilerini çek (Adres ve ID için)
    if (this.authService.isLoggedIn()) {
      this.customerService.getProfile().subscribe(profile => {
        if (profile) {
          this.customerId = profile.id;
          this.address.city = profile.city || '';
          this.address.fullAddress = profile.address || '';
          this.cdr.detectChanges();
        }
      });
    }
  }

  completeOrder() {
    if (!this.customerId) {
      alert("Sipariş oluşturmak için müşteri profili bulunamadı.");
      return;
    }

    if (!this.card.number || this.card.number.length < 16) {
      alert("Lütfen geçerli bir kart numarası giriniz.");
      return;
    }

    this.isLoading = true;

    // A. Backend'in Beklediği OrderCreateDto Formatı
    const orderDto = {
      customerId: this.customerId,
      // Şirket ID'si genelde ürünün şirketinden gelir ama 
      // Çoklu satıcı (Marketplace) yapısında her ürün farklı şirketten olabilir.
      // Basitlik adına ilk ürünün şirketini veya sabit bir ID alıyoruz:
      companyId: this.cartService.cartItems()[0].product.companyId, 
      
      orderItems: this.cartService.cartItems().map(item => ({
        productId: item.product.id,
        quantity: item.quantity,
        price: item.product.price,
        productName: item.product.name 
      }))
    };

    // B. Simüle Edilmiş Banka Gecikmesi (1.5 Saniye)
    setTimeout(() => {
      
      // C. Siparişi Backend'e Gönder
      this.orderService.createOrder(orderDto).subscribe({
        next: (res) => {
          this.isLoading = false;
          if (res.success) {
            // Başarılı!
            this.cartService.clearCart(); // Sepeti temizle
            alert("Siparişiniz başarıyla alındı! 🎉");
            this.router.navigate(['/orders']); // Siparişlerim sayfasına git
          } else {
            alert(res.message);
          }
        },
        error: (err) => {
          console.error(err);
          this.isLoading = false;
          alert("Sipariş oluşturulurken bir hata oluştu.");
        }
      });
         this.cdr.detectChanges();
    }, 1500);
  }
}