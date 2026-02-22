import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CategoryService } from '../../core/services/categoryService';
import { Category } from '../../core/models/category';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './category-list.html',
  styleUrls: ['./category-list.scss'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class CategoryList implements OnInit {
  
  categories: Category[] = [];
  loading = true;

  constructor(
    private categoryService: CategoryService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({
      next: (res) => {
        this.categories = res.data || [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => this.loading = false
    });
     this.cdr.detectChanges();
  }

  // Kategori ismine göre ikon belirleyen yardımcı metod
  getCategoryIcon(name: string): string {
    const lowerName = name.toLowerCase();
    if (lowerName.includes('telefon')) return 'bi-phone';
    if (lowerName.includes('bilgisayar')) return 'bi-laptop';
    if (lowerName.includes('tv') || lowerName.includes('ses')) return 'bi-tv';
    if (lowerName.includes('ev') || lowerName.includes('süpürge')) return 'bi-house-heart';
    if (lowerName.includes('aksesuar')) return 'bi-headphones';
    if (lowerName.includes('oyun')) return 'bi-controller';
    if (lowerName.includes('ziraat')) return 'bi-tree';
    return 'bi-grid'; // Varsayılan
  }

  // Kategori ismine göre renk belirleyen yardımcı metod (Opsiyonel Görsellik)
  getCategoryColor(index: number): string {
    const colors = ['bg-primary', 'bg-success', 'bg-warning text-dark', 'bg-info text-dark', 'bg-danger', 'bg-secondary'];
    return colors[index % colors.length];
  }
  
}