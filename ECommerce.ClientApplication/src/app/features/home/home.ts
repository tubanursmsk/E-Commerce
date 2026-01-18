
import { Component, OnInit, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BannerService } from '../../core/services/bannerService';
import { Banner } from '../../core/models/banner';

declare var bootstrap: any; // Bootstrap'i global tanımlıyoruz

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './home.html',
    styleUrls: ['./home.scss']
})
export class HomeComponent implements OnInit, AfterViewInit {
    banners: Banner[] = [];

    constructor(private bannerService: BannerService) { }

    ngOnInit(): void {
        this.bannerService.getBanners().subscribe({
            next: (data) => {
                this.banners = data;

                // Veri DOM'a basıldıktan hemen sonra Carousel ayarlarını yapalım
                setTimeout(() => {
                    const carouselElement = document.querySelector('#heroCarousel');
                    if (carouselElement) {
                        new bootstrap.Carousel(carouselElement, {
                            interval: 3000, // 3 saniye
                            ride: 'carousel', // Sayfa açılır açılmaz başla
                            pause: 'hover' // Kullanıcı mouse ile üzerine gelince durur (Vatan stili)
                        });
                    }
                }, 100);
            }
        });
    }

    ngAfterViewInit(): void {
        // Sayfa ilk yüklendiğinde de deneyelim
    }
}
