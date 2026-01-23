import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService, CartItem } from '../../core/services/cartService';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart.html',
  styleUrls: ['./cart.scss']
})
export class Cart {
  constructor(public cartService: CartService) {}

  // Miktar Azalt (- Butonu)
  decrease(item: CartItem) {
    if (item.quantity > 1) {
      item.quantity--;
      this.cartService.updateStorage(); // Servis'e bu metodu ekleyeceğiz
    } else {
      this.cartService.removeFromCart(item.product.id);
    }
  }

  // Miktar Artır (+ Butonu)
  increase(item: CartItem) {
    item.quantity++;
    this.cartService.updateStorage();
  }

  // Sil (Çöp Kutusu)
  remove(id: string) {
    if(confirm('Ürünü sepetten çıkarmak istiyor musunuz?')) {
      this.cartService.removeFromCart(id);
    }
  }
}