import { Injectable } from '@angular/core';
import { BaseService } from './baseService';
import { Observable, map } from 'rxjs';
import { Product, ProductFilterParams, ProductListResponse } from '../models/product';
import { ApiResponse } from '../models/apiResponse';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private baseService: BaseService) { }

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

  // GELİŞMİŞ FİLTRELEME 
  getFilteredProducts(filter: ProductFilterParams): Observable<ProductListResponse> {

    // Parametreleri Query String'e (URL'e) çeviriyoruz
    let params = new HttpParams();

    if (filter.categoryId) params = params.append('CategoryId', filter.categoryId);

    // Dizi (Array) tipindeki verileri ekleme (Çoklu Marka Seçimi)
    if (filter.brandIds && filter.brandIds.length > 0) {
      filter.brandIds.forEach(id => {
        params = params.append('BrandIds', id);
      });
    }

    if (filter.minPrice) params = params.append('MinPrice', filter.minPrice);
    if (filter.maxPrice) params = params.append('MaxPrice', filter.maxPrice);
    if (filter.keyword) params = params.append('Keyword', filter.keyword);

    if (filter.isFreeShipping) params = params.append('IsFreeShipping', true);
    if (filter.isFastDelivery) params = params.append('IsFastDelivery', true);

    if (filter.sortBy) params = params.append('SortBy', filter.sortBy);

    params = params.append('PageNumber', filter.pageNumber || 1);
    params = params.append('PageSize', filter.pageSize || 12);

    // BaseService genellikle URL string alır. Params'ı stringe çevirip ekliyoruz.
    // Eğer BaseService'in params desteği varsa onu da kullanabilirsin ama bu yöntem garanti çalışır.
    const queryString = params.toString();

    return this.baseService.get<ApiResponse<ProductListResponse>>(`Product/Filter?${queryString}`).pipe(
      map(response => response.data)
    );
  }
}