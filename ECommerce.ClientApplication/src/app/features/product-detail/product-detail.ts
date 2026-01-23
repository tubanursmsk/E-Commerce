import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../core/models/product';
import { ProductService } from '../../core/services/productService';
import { ReviewService } from '../../core/services/reviewService';
import { Review } from '../../core/models/review';
import { CartService } from '../../core/services/cartService';
import { FavoriteService } from '../../core/services/favoriteService';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-detail.html',
  styleUrls: ['./product-detail.scss'],
  changeDetection: ChangeDetectionStrategy.Default
})

export class ProductDetail implements OnInit {
  product?: Product;
  reviews: Review[] = []; // Yorumları tutacak dizi
  averageRating: number = 0; // Ortalama puan
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private reviewService: ReviewService, // Servisi inject ettik
     private cartService: CartService,         // Inject et
    public favoriteService: FavoriteService,  // HTML'den erişmek için Public Inject et
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadData(id);
    }
  }

  loadData(id: string) {
    // 1. Ürün Detayını Çek
    this.productService.getProductById(id).subscribe({
      next: (data) => {
        this.product = data;
        
        // 2. Ürün geldikten sonra Yorumları Çek
        this.loadReviews(id);
      },
      error: () => this.loading = false
    });
  }

  loadReviews(productId: string) {
    this.reviewService.getReviewsByProductId(productId).subscribe({
      next: (data) => {
        this.reviews = data;
        this.calculateAverageRating();
        this.loading = false;
        this.cdr.detectChanges(); // Veri geldiğinde ekranı yenile
      },
      error: () => this.loading = false
    });
  }

  calculateAverageRating() {
    if (this.reviews.length === 0) {
      this.averageRating = 0;
      return;
    }
    const total = this.reviews.reduce((sum, review) => sum + review.rating, 0);
    this.averageRating = total / this.reviews.length;
  }
  
  // Yıldızları döngüyle oluşturmak için yardımcı metod (HTML'de kullanacağız)
  getStarArray(rating: number): number[] {
    return Array(rating).fill(0);
  }

  scrollToTabs() {
  const tabElement = document.getElementById('comments-tab');
  const commentsSection = document.getElementById('comments');
  
  if (tabElement && commentsSection) {
    // 1. Yorumlar sekmesini aktif et (Bootstrap JS simülasyonu)
    tabElement.click(); 
    
    // 2. Oraya yumuşakça kaydır
    tabElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }
}
 addToCart() {
    if (this.product) {
      this.cartService.addToCart(this.product);
      alert("Ürün sepete eklendi!"); // Şimdilik basit alert, sonra Toast ekleriz
    }
  }

  // FAVORİ EKLE/ÇIKAR METODU
  toggleFav() {
    if (this.product) {
      this.favoriteService.toggleFavorite(this.product);
    }
  }
}