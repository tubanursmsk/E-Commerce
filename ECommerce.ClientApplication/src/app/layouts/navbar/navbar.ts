import { Component, OnInit ,ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router'; // Router Eklendi
import { FormsModule } from '@angular/forms'; // FormsModule Eklendi
import { AuthService } from '../../core/services/authService';
import { CartService } from '../../core/services/cartService';
//import { CategoryService } from '../../core/services/categoryService';
import { Category } from '../../core/models/category';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule], 
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.scss']
})
export class Navbar {
  
  searchKeyword: string = ''; // Arama metnini tutacak değişken
  categories: Category[] = []; // Kategorileri tutacak dizi

  constructor(
    public authService: AuthService, 
    public cartService: CartService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  onSearch() {
    if (this.searchKeyword && this.searchKeyword.trim().length > 0) {
      // Ürünler sayfasına 'keyword' parametresiyle git
      this.router.navigate(['/products'], { 
        queryParams: { keyword: this.searchKeyword } 
      });
      // Arama sonrası kutuyu temizlemek istersen:
       this.searchKeyword = '';
        this.cdr.detectChanges(); 
    }
  }
}

/*

Uygulama /products?keyword=aranan_kelime sayfasına yönlenecek.
ProductListComponent zaten URL'deki keyword parametresini dinliyor (önceki adımda kodlamıştık), 
bu yüzden arama sonucunu otomatik olarak gösterecek.

*/