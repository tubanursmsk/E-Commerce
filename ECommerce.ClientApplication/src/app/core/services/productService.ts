import { Injectable } from '@angular/core';
import { BaseService } from './baseService';
import { Observable, map } from 'rxjs';
import { Product } from '../models/product';
import { ApiResponse } from '../models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private baseService: BaseService) {}

  // Ana sayfa için en yeni veya öne çıkan ürünleri getir
  getFeaturedProducts(): Observable<Product[]> {
    return this.baseService.get<ApiResponse<Product[]>>('Product/Featured').pipe(
      map(response => response.data)
    );
  }

  getProductById(id: string): Observable<Product> {
    return this.baseService.get<ApiResponse<Product>>(`Product/GetById/${id}`).pipe(
      map(response => response.data)
    );
  }
}