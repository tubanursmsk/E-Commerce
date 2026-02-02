import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { OrderService } from '../../core/services/orderService';
import { ImageUrlPipe } from '../../core/pipes/image-url-pipe';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ImageUrlPipe],
  templateUrl: './orders-detail.html',
  styleUrls: ['./orders-detail.scss'],
  changeDetection: ChangeDetectionStrategy.Default
})
export class OrdersDetail implements OnInit {
  order: any;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private orderService: OrderService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.orderService.getOrderById(id).subscribe({
        next: (res) => {
          this.order = res.data;
          this.loading = false;
          this.cdr.detectChanges();
        },
        error: () => this.loading = false
      });
    }
  }

  // Sipariş Durumuna Göre Progress Bar Class'ı
  getStatusClass(currentStatus: number, stepStatus: number): string {
    if (currentStatus >= stepStatus) return 'active';
    return '';
  }
}