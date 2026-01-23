import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  // Sepetteki ürün sayısı
  itemCount = signal<number>(0);

  addToCart() {
    this.itemCount.update(count => count + 1);
  }
}