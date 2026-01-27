import { Component, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FavoriteService } from '../../core/services/favoriteService';
import { CartService } from '../../core/services/cartService';
import { Product } from '../../core/models/product';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './favorites.html',
  changeDetection: ChangeDetectionStrategy.Default
})
export class Favorites {
  constructor(
    public favoriteService: FavoriteService,
    private cartService: CartService,
    private cdr: ChangeDetectorRef
  ) {}

  moveToCart(product: Product) {
    this.cartService.addToCart(product);
    // İsteğe bağlı: Sepete ekleyince favorilerden silsin mi? 
    // this.favService.toggleFavorite(product); 
    alert("Ürün sepete eklendi!");
    this.cdr.markForCheck();
  }
}