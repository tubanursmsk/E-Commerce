import { Injectable, signal, computed } from '@angular/core';
import { Product } from '../models/product';

// Sepet Elemanı Modeli
export interface CartItem {
  product: Product;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  updateStorage() {
    throw new Error('Method not implemented.');
  }
  // Sepetteki ürünleri tutan sinyal
  cartItems = signal<CartItem[]>([]);

  // Toplam ürün sayısını otomatik hesaplayan sinyal (Navbar için)
  totalItems = computed(() => this.cartItems().reduce((acc, item) => acc + item.quantity, 0));

  // Toplam tutarı hesaplayan sinyal
  totalPrice = computed(() => this.cartItems().reduce((acc, item) => acc + (item.product.price * item.quantity), 0));

  constructor() {
    // Uygulama açılınca localStorage'dan sepeti geri yükle
    const storedCart = localStorage.getItem('cart');
    if (storedCart) {
      this.cartItems.set(JSON.parse(storedCart));
    }
  }

  addToCart(product: Product) {
    const currentItems = this.cartItems();
    const existingItem = currentItems.find(item => item.product.id === product.id);

    if (existingItem) {
      // Ürün zaten varsa miktarını artır
      existingItem.quantity += 1;
      this.cartItems.set([...currentItems]); // Sinyali tetikle
    } else {
      // Yoksa yeni ekle
      this.cartItems.set([...currentItems, { product, quantity: 1 }]);
    }
    
    this.saveToStorage();
  }

  removeFromCart(productId: string) {
    const currentItems = this.cartItems().filter(item => item.product.id !== productId);
    this.cartItems.set(currentItems);
    this.saveToStorage();
  }

  clearCart() {
    this.cartItems.set([]);
    this.saveToStorage();
  }

  private saveToStorage() {
    localStorage.setItem('cart', JSON.stringify(this.cartItems()));
  }
}