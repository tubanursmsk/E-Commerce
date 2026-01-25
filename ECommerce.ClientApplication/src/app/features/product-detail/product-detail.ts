import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../core/models/product';
import { ProductService } from '../../core/services/productService';
import { ReviewService } from '../../core/services/reviewService';
import { Review } from '../../core/models/review';
import { CartService } from '../../core/services/cartService';
import { FavoriteService } from '../../core/services/favoriteService';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/authService';
import { CustomerService } from '../../core/services/customerService';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-detail.html',
  styleUrls: ['./product-detail.scss'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class ProductDetail implements OnInit {
  product?: Product;
  reviews: Review[] = [];
  averageRating: number = 0;
  loading = true;

  // Yorum Formu Verisi
  newReview = {
    rating: 5,
    comment: ''
  };

  isSubmitting = false;
  currentCustomerId: string | null = null; // Giriş yapan kullanıcının Müşteri ID'si

  // Düzenleme Modu Değişkenleri
  isEditing = false;
  editingReviewId: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private reviewService: ReviewService,
    private cartService: CartService,
    public favoriteService: FavoriteService,
    private cdr: ChangeDetectorRef,
    public authService: AuthService,
    private customerService: CustomerService
  ) { }

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

        // 3. Eğer giriş yapmışsa Profil bilgisini (Customer ID) çek
        if (this.authService.isLoggedIn()) {
          this.customerService.getProfile().subscribe(res => {
            if (res) this.currentCustomerId = res.id;
          });
        }
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
        this.cdr.detectChanges();
      },
      error: () => this.loading = false
    });
  }

  // --- DÜZENLEME VE SİLME İŞLEMLERİ ---

  // Düzenleme Modunu Açan Fonksiyon
  openEditModal(review: Review) {
    this.isEditing = true;
    this.editingReviewId = review.id;

    // Mevcut yorum verilerini forma doldur
    this.newReview = {
      rating: review.rating,
      comment: review.comment
    };

    // Modalı aç (Bootstrap JS simülasyonu)
    // HTML'deki "Yorum Yap" butonuna tıklamak yerine, oluşturduğumuz gizli butona tıklıyoruz.
    // Böylece "isEditing = false" yapan kod çalışmıyor!
    const modalBtn = document.getElementById('hiddenModalTrigger');
    if (modalBtn) {
      modalBtn.click();
    }
  }

  // Yorum Silme Fonksiyonu
  deleteReview(reviewId: string) {
    if (confirm("Bu yorumu silmek istediğinize emin misiniz?")) {
      this.reviewService.deleteReview(reviewId).subscribe({
        next: (res) => {
          if (res.success) {
            alert("Yorum silindi.");
            if (this.product) this.loadReviews(this.product.id);
          } else {
            alert(res.message);
          }
        },
        error: (err) => console.error(err)
      });
    }
  }

  // --- FORM GÖNDERME (EKLEME & GÜNCELLEME) ---
  submitReview() {
    if (!this.newReview.comment || this.newReview.comment.length < 10) {
      alert("Yorumunuz en az 10 karakter olmalıdır.");
      return;
    }

    this.isSubmitting = true;

    this.customerService.getProfile().subscribe({
      next: (profileData) => {
        if (!profileData || !profileData.id) {
          alert("Profil hatası.");
          this.isSubmitting = false;
          return;
        }

        // Ortak DTO verisi
        const reviewDto = {
          productId: this.product?.id,
          customerId: profileData.id,
          rating: this.newReview.rating,
          comment: this.newReview.comment,
          // DİKKAT: Backend Update DTO'su ID istiyor mu? 
          // Eğer ReviewUpdateDto içinde Id propertysi yoksa buraya koyma.
          // Genelde ID URL'den gider: api/Review/Update/{id}
        };

        // ---------- AYRIM NOKTASI --------
        if (this.isEditing && this.editingReviewId) {
          // *** GÜNCELLEME İŞLEMİ ***
          console.log("Güncelleniyor...", this.editingReviewId);

          // Update DTO'su genellikle ID istemez (URL'den alır) ve CustomerId/ProductId değişmez.
          // Sadece değişen alanları gönderelim.
          const updateDto = {
            rating: this.newReview.rating,
            comment: this.newReview.comment,
            status: true // Backend status bekliyorsa
          };

          this.reviewService.updateReview(this.editingReviewId, updateDto).subscribe({
            next: (res) => {
              // Backend bool döndüğü için success kontrolü şöyle olabilir:
              // Eğer ApiResponse<bool> dönüyorsa: res.success
              this.handleSuccess("Yorum başarıyla güncellendi!");
            },
            error: (err) => {
              console.error("Güncelleme hatası:", err);
              this.handleError();
            }
          });

        } else {
          // *** YENİ KAYIT İŞLEMİ ***
          console.log("Yeni kayıt oluşturuluyor...");

          const createDto = {
            productId: this.product?.id,
            customerId: profileData.id,
            rating: this.newReview.rating,
            comment: this.newReview.comment
          };

          this.reviewService.createReview(createDto).subscribe({
            next: (res) => this.handleSuccess("Yorumunuz eklendi!"),
            error: (err) => this.handleError()
          });
        }
      },
      error: () => {
        alert("Profil bilgisi alınamadı.");
        this.isSubmitting = false;
      }
    });
  }

  // --- YARDIMCI METODLAR ---

  // Başarılı işlem sonrası temizlik yapan metod
  handleSuccess(msg: string) {
    alert(msg);
    this.isSubmitting = false;

    // Modu sıfırla
    this.isEditing = false;
    this.editingReviewId = null;
    this.newReview = { rating: 5, comment: '' };

    // Modalı kapat
    const closeBtn = document.getElementById('closeModalBtn');
    if (closeBtn) closeBtn.click();

    // Listeyi yenile
    if (this.product) this.loadReviews(this.product.id);
  }

  // Hata durumunda çalışan metod
  handleError() {
    alert("İşlem sırasında bir hata oluştu.");
    this.isSubmitting = false;
  }

  calculateAverageRating() {
    if (this.reviews.length === 0) {
      this.averageRating = 0;
      return;
    }
    const total = this.reviews.reduce((sum, review) => sum + review.rating, 0);
    this.averageRating = total / this.reviews.length;
  }

  getStarArray(rating: number): number[] {
    return Array(rating).fill(0);
  }

  setRating(star: number) {
    this.newReview.rating = star;
  }

  scrollToTabs() {
    const tabElement = document.getElementById('comments-tab');
    const commentsSection = document.getElementById('comments');

    if (tabElement && commentsSection) {
      tabElement.click();
      tabElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }

  addToCart() {
    if (this.product) {
      this.cartService.addToCart(this.product);
      alert("Ürün sepete eklendi!");
    }
  }

  toggleFav() {
    if (this.product) {
      this.favoriteService.toggleFavorite(this.product);
    }
  }
}