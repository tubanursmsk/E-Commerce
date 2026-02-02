import { Component, OnInit, AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../core/services/orderService';
import { CustomerService } from '../../core/services/customerService';
import { RouterModule } from '@angular/router';
import { ImageUrlPipe } from '../../core/pipes/image-url-pipe';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterModule, ImageUrlPipe],
  templateUrl: './orders.html',
  changeDetection: ChangeDetectionStrategy.Default

})
export class OrdersComponent implements OnInit {
  orders: any[] = [];
  loading = true;

  constructor(
    private cdr: ChangeDetectorRef,
    private orderService: OrderService,
    private customerService: CustomerService
  ) { }

  ngOnInit(): void {
    // Önce Müşteri ID'sini al, sonra siparişleri çek
    this.customerService.getProfile().subscribe(profile => {
      if (profile && profile.id) {
        this.orderService.getOrdersByCustomer(profile.id).subscribe({
          next: (res) => {
            this.orders = res.data || [];
            this.loading = false;
            this.cdr.detectChanges();
          },
          error: () => this.loading = false
        });
      }
    });
  }
}