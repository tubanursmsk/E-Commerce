import { Injectable, signal } from '@angular/core';
import { Product } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class FavoriteService {
  // Favori ürünlerin listesi
  favorites = signal<Product[]>([]);

  constructor() {
    const storedFavs = localStorage.getItem('favorites');
    if (storedFavs) {
      this.favorites.set(JSON.parse(storedFavs));
    }
  }

  toggleFavorite(product: Product) {
    const currentFavs = this.favorites();
    const exists = currentFavs.find(p => p.id === product.id);

    if (exists) {
      // Varsa çıkar
      this.favorites.set(currentFavs.filter(p => p.id !== product.id));
    } else {
      // Yoksa ekle
      this.favorites.set([...currentFavs, product]);
    }
    
    localStorage.setItem('favorites', JSON.stringify(this.favorites()));
  }

  isFavorite(productId: string): boolean {
    return this.favorites().some(p => p.id === productId);
  }
}