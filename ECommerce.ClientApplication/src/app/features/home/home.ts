import { Component, OnInit, AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BannerService } from '../../core/services/bannerService'; // Dosya isminizle eşleştiğinden emin olun
import { ProductService } from '../../core/services/productService';
import { Banner } from '../../core/models/banner';
import { Product } from '../../core/models/product';
import { RouterModule } from '@angular/router';
import { CartService } from '../../core/services/cartService';
import { CategoryService } from '../../core/services/categoryService';
import { Category } from '../../core/models/category';

declare var bootstrap: any;

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './home.html',
    styleUrls: ['./home.scss'],
    changeDetection: ChangeDetectionStrategy.Default
})
export class HomeComponent implements OnInit, AfterViewInit {
    banners: Banner[] = [];
    products: Product[] = [];
    categories: Category[] = [];
    cat: any;

    constructor(
        private cdr: ChangeDetectorRef,
        private bannerService: BannerService,
        private productService: ProductService,
        private categoryService: CategoryService,
        private cartService: CartService) { }

    ngOnInit(): void {
        // Ürünleri Çek 
        this.productService.getFeaturedProducts().subscribe({
            next: (data) => {
                console.log("API'den gelen Ürün verisi:", data);
                this.products = data;
                this.cdr.detectChanges(); // Değişiklikleri algıla
            },
            error: (err) => console.error("Ürün servisi hatası:", err)
        });

        // Bannerları Çek
        this.bannerService.getBanners().subscribe({
            next: (data) => {
                this.banners = data;
                this.initCarousel(); // Ayrı bir metoda aldık
                this.cdr.detectChanges();
            },
            error: (err) => console.error("Banner servisi hatası:", err)
        });

        // Kategorileri çek (Navbar'daki gibi)
        this.categoryService.getCategories().subscribe({
            next: (res) => {
                this.categories = res.data || [];
                this.cdr.detectChanges();
            }
        });

    }

    // Navbar'daki ikon fonksiyonunun aynısını buraya koyuyoruz
    getCategoryIcon(name: string): string {
        const lowerName = name.toLowerCase();
        if (lowerName.includes('telefon')) return 'bi-phone';
        if (lowerName.includes('bilgisayar')) return 'bi-laptop';
        if (lowerName.includes('tv') || lowerName.includes('ses')) return 'bi-tv';
        if (lowerName.includes('ev') || lowerName.includes('alet')) return 'bi-house-heart';
        if (lowerName.includes('aksesuar')) return 'bi-headphones';
        if (lowerName.includes('kamera')) return 'bi-camera';
        if (lowerName.includes('saat')) return 'bi-watch';
        return 'bi-grid';
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
    // Dummy Marka Logoları (Gerçek logolar yerine placeholder veya CDN linkleri kullanılabilir)
    // Şimdilik temsili logolar kullanıyorum.
    brands = [
        { name: 'Apple', logo: 'https://upload.wikimedia.org/wikipedia/commons/f/fa/Apple_logo_black.svg' },
        { name: 'Samsung', logo: 'https://cdn.worldvectorlogo.com/logos/samsung-8.svg' },
        { name: 'Xiaomi', logo: 'https://upload.wikimedia.org/wikipedia/commons/a/ae/Xiaomi_logo_%282021-%29.svg' },
        { name: 'Huawei', logo: 'https://www.logo.wine/a/logo/Huawei/Huawei-Vertical-Logo.wine.svg' },
        { name: 'Lenovo', logo: 'https://upload.wikimedia.org/wikipedia/commons/b/b8/Lenovo_logo_2015.svg' },
        { name: 'Asus', logo: 'https://upload.wikimedia.org/wikipedia/commons/2/2e/ASUS_Logo.svg' },
        { name: 'Dell', logo: 'https://upload.wikimedia.org/wikipedia/commons/4/48/Dell_Logo.svg' },
        { name: 'HP', logo: 'https://upload.wikimedia.org/wikipedia/commons/a/ad/HP_logo_2012.svg' },
        { name: 'Sony', logo: 'https://download.logo.wine/logo/Sony_Professional_Solutions/Sony_Professional_Solutions-Logo.wine.png' },
        { name: 'LG', logo: 'https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/LG_logo_%282014%29.svg/960px-LG_logo_%282014%29.svg.png' }
    ];
}


