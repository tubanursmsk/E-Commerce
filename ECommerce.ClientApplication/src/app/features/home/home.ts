import { Component, OnInit, AfterViewInit,ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BannerService } from '../../core/services/bannerService'; // Dosya isminizle eşleştiğinden emin olun
import { ProductService } from '../../core/services/productService'; 
import { Banner } from '../../core/models/banner';
import { Product } from '../../core/models/product';
import { RouterModule } from '@angular/router';
import { CartService } from '../../core/services/cartService';

declare var bootstrap: any;

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [CommonModule,RouterModule],
    templateUrl: './home.html',
    styleUrls: ['./home.scss'],
    changeDetection: ChangeDetectionStrategy.Default
})
export class HomeComponent implements OnInit, AfterViewInit {
    banners: Banner[] = [];
    products: Product[] = [];

    constructor(
        private cdr: ChangeDetectorRef,
        private bannerService: BannerService,
        private productService: ProductService,
        private cartService: CartService ) { }

    ngOnInit(): void {
        // Ürünleri Çek 
        this.productService.getFeaturedProducts().subscribe({
            next: (data) => {
                console.log("API'den gelen Ürün verisi:", data);
                this.products = data;
            },
            error: (err) => console.error("Ürün servisi hatası:", err)
        });

        // Bannerları Çek
        this.bannerService.getBanners().subscribe({
            next: (data) => {
                this.banners = data;
                this.initCarousel(); // Ayrı bir metoda aldık
            },
            error: (err) => console.error("Banner servisi hatası:", err)
        });

        
    }
     addToCart(product: Product) {
    if (product) {
      this.cartService.addToCart(product);
      alert("Ürün sepete eklendi!"); // Şimdilik basit alert, sonra Toast ekleriz
    }
  }


    private initCarousel() {
        setTimeout(() => {
            const carouselElement = document.querySelector('#heroCarousel');
            if (carouselElement && typeof bootstrap !== 'undefined') {
                const carousel = new bootstrap.Carousel(carouselElement, {
                    interval: 3000,
                    ride: 'carousel',
                    pause: 'hover'
                });
                carousel.cycle(); // Manuel olarak döngüyü başlat
            }
             this.cdr.detectChanges();
        }, 300); // Süreyi biraz artırdık ki DOM tam yerleşsin
    }

    ngAfterViewInit(): void { }

    
}