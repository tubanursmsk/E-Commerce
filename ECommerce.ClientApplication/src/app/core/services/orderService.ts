import { Component, OnInit, AfterViewInit,ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { Injectable } from '@angular/core';
import { BaseService } from './baseService';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private baseService: BaseService) { }

  // Sipariş Oluştur
  createOrder(orderData: any): Observable<ApiResponse<string>> {
    return this.baseService.post<ApiResponse<string>>('Order/Create', orderData);
  }
  // Müşterinin Siparişlerini Getir
  getOrdersByCustomer(customerId: string): Observable<ApiResponse<any[]>> {
    return this.baseService.get<ApiResponse<any[]>>(`Order/ByCustomer/${customerId}`);
  }
  
  getOrderById(id: string): Observable<ApiResponse<any>> {
  return this.baseService.get<ApiResponse<any>>(`Order/${id}`);
}
}