/*

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
  reviews: Review[] = []; // Yorumları tutacak dizi
  averageRating: number = 0; // Ortalama puan
  loading = true;
  newReview = {
    rating: 5,
    comment: ''
  };
  isSubmitting = false;
  currentCustomerId: string | null = null;  // YENİ: Giriş yapan müşterinin ID'si (Kıyaslama için)
  isEditing = false;  // Düzenleme modu kontrolü
  editingReviewId: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private reviewService: ReviewService, // Servisi inject ettik
    private cartService: CartService,         // Inject et
    public favoriteService: FavoriteService,  // HTML'den erişmek için Public Inject et
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
        // YENİ: Eğer giriş yapmışsa Customer ID'sini öğren
        if (this.authService.isLoggedIn()) {
          this.customerService.getProfile().subscribe(res => {
            if (res) this.currentCustomerId = res.id;
          });
        }
      },
      error: () => this.loading = false
    });
  }

  // YENİ: Düzenleme Modunu Aç
  openEditModal(review: Review) {
    this.isEditing = true;
    this.editingReviewId = review.id;
    
    // Mevcut yorum verilerini forma doldur
    this.newReview = {
      rating: review.rating,
      comment: review.comment
    };
    
    // Modalı aç (Bootstrap JS simülasyonu)
    const modalBtn = document.querySelector('[data-bs-target="#reviewModal"]') as HTMLElement;
    if(modalBtn) modalBtn.click();
  }

  // Silme İşlemi
  deleteReview(reviewId: string) {
    if(confirm("Bu yorumu silmek istediğinize emin misiniz?")) {
      this.reviewService.deleteReview(reviewId).subscribe({
        next: (res) => {
          if(res.success) {
            alert("Yorum silindi.");
            if(this.product) this.loadReviews(this.product.id);
          } else {
            alert(res.message);
          }
        }
      });
    }
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
  // YENİ: Modalda yıldız seçimi
  setRating(star: number) {
    this.newReview.rating = star;
  }

  // Yorum Gönderme Fonksiyonu
  submitReview() {
    if (!this.authService.isLoggedIn()) {
      alert("Yorum yapmak için lütfen giriş yapınız.");
      return;
    }

    if (!this.newReview.comment || this.newReview.comment.length < 10) {
      alert("Yorumunuz en az 10 karakter olmalıdır.");
      return;
    }

    this.isSubmitting = true;

    this.customerService.getProfile().subscribe({
      next: (profileData: { id: string }) => {
        // KONTROL: Eğer profileData var ama ID'si boşsa (0000...) veya yoksa
        if (!profileData || !profileData.id || profileData.id === '00000000-0000-0000-0000-000000000000') {
          alert("Yorum yapabilmek için lütfen önce Profil sayfasından bilgilerinizi kaydediniz.");
          this.isSubmitting = false;
          // İstersen kullanıcıyı profil sayfasına yönlendirebilirsin:
          // this.router.navigate(['/profile']);
          return;
        }

        // Veri varsa işleme devam et
        const reviewDto = {
          productId: this.product?.id,
          customerId: profileData.id, // Artık geçerli bir ID olduğundan eminiz
          rating: this.newReview.rating,
          comment: this.newReview.comment
        };

        // 3. Adım: Yorumu gönder
        this.reviewService.createReview(reviewDto).subscribe({
          next: (res) => {
            if (res.success) {
              alert("Yorumunuz başarıyla eklendi!");

              // Formu temizle ve modalı kapat (Bootstrap JS kullanmadan basitçe)
              this.newReview = { rating: 5, comment: '' };
              const closeBtn = document.getElementById('closeModalBtn');
              if (closeBtn) closeBtn.click();

              // Yorumları yeniden yükle
              if (this.product?.id) this.loadReviews(this.product.id);
            } else {
              alert(res.message);
            }
            this.isSubmitting = false;
          },
          error: (err) => {
            console.error(err);
            alert("Yorum gönderilirken hata oluştu.");
            this.isSubmitting = false;
          }
        });
      },
      error: () => {
        alert("Müşteri kaydınız bulunamadı. Lütfen önce profil bilgilerinizi kaydediniz.");
        this.isSubmitting = false;
      }
    });
  }
}

*/

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
    const modalBtn = document.querySelector('[data-bs-target="#reviewModal"]') as HTMLElement;
    if(modalBtn) modalBtn.click();
  }

  // Yorum Silme Fonksiyonu
  deleteReview(reviewId: string) {
    if(confirm("Bu yorumu silmek istediğinize emin misiniz?")) {
      this.reviewService.deleteReview(reviewId).subscribe({
        next: (res) => {
          if(res.success) {
            alert("Yorum silindi.");
            if(this.product) this.loadReviews(this.product.id);
          } else {
            alert(res.message);
          }
        },
        error: (err) => console.error(err)
      });
    }
  }

  // --- FORM GÖNDERME (EKLEME & GÜNCELLEME) ---
/*
  submitReview() {
    // 1. Giriş Kontrolü
    if (!this.authService.isLoggedIn()) {
      alert("Yorum yapmak için lütfen giriş yapınız.");
      return;
    }

    // 2. Validasyon
    if (!this.newReview.comment || this.newReview.comment.length < 10) {
      alert("Yorumunuz en az 10 karakter olmalıdır.");
      return;
    }

    this.isSubmitting = true;

    // 3. Profil Bilgisini (Customer ID) Al
    this.customerService.getProfile().subscribe({
      next: (profileData: { id: string }) => {
        
        // Profil ID kontrolü
        if (!profileData || !profileData.id || profileData.id === '00000000-0000-0000-0000-000000000000') {
          alert("Yorum yapabilmek için lütfen önce Profil sayfasından bilgilerinizi kaydediniz.");
          this.isSubmitting = false;
          return;
        }

        // Gönderilecek Veri Paketi (DTO)
        const reviewDto = {
          productId: this.product?.id,
          customerId: profileData.id,
          rating: this.newReview.rating,
          comment: this.newReview.comment,
          // Eğer düzenleme modundaysak ID'yi ekle
          id: this.isEditing ? this.editingReviewId : undefined 
        };

        // 4. Karar Anı: Güncelleme mi, Yeni Kayıt mı?
        if (this.isEditing && this.editingReviewId) {
          // --- GÜNCELLEME ---
          this.reviewService.updateReview(this.editingReviewId, reviewDto).subscribe({
            next: (res) => {
              if (res.success) this.handleSuccess("Yorum güncellendi!");
              else { alert(res.message); this.isSubmitting = false; }
            },
            error: () => this.handleError()
          });
        } else {
          // --- YENİ KAYIT ---
          this.reviewService.createReview(reviewDto).subscribe({
            next: (res) => {
              if (res.success) this.handleSuccess("Yorum eklendi!");
              else { alert(res.message); this.isSubmitting = false; }
            },
            error: () => this.handleError()
          });
        }
      },
      error: () => {
        alert("Müşteri kaydınız bulunamadı. Lütfen profil sayfasını kontrol ediniz.");
        this.isSubmitting = false;
      }
    });
  }*/

    // submitReview Metodunu şu şekilde güncelle:

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

      if (this.isEditing && this.editingReviewId) {
        // --- GÜNCELLEME MODU ---
        console.log("Güncelleniyor...", this.editingReviewId);
        
        this.reviewService.updateReview(this.editingReviewId, reviewDto).subscribe({
          next: (res) => {
            if (res.success) {
              this.handleSuccess("Yorum başarıyla güncellendi!");
            } else {
              alert(res.message);
              this.isSubmitting = false;
            }
          },
          error: (err) => {
            console.error("Güncelleme hatası:", err);
            this.handleError();
          }
        });

      } else {
        // --- YENİ KAYIT MODU ---
        console.log("Yeni kayıt oluşturuluyor...");
        
        this.reviewService.createReview(reviewDto).subscribe({
          next: (res) => {
            if (res.success) {
              this.handleSuccess("Yorumunuz eklendi!");
            } else {
              alert(res.message);
              this.isSubmitting = false;
            }
          },
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
    if(closeBtn) closeBtn.click();
    
    // Listeyi yenile
    if(this.product) this.loadReviews(this.product.id);
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