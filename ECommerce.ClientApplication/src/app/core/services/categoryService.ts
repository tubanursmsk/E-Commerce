import { Injectable } from '@angular/core';
import { BaseService } from './baseService';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/apiResponse';
import { Category } from '../models/category';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(private baseService: BaseService) { }

  // Tüm kategorileri getir
  // Backend'deki endpoint muhtemelen: api/Category/List
  getCategories(): Observable<ApiResponse<Category[]>> {
    return this.baseService.get<ApiResponse<Category[]>>('Category/AllList');
  }

  // Tek bir kategori getir (İleride lazım olabilir)
  getCategoryById(id: string): Observable<ApiResponse<Category>> {
    return this.baseService.get<ApiResponse<Category>>(`Category/GetById/${id}`);
  }
}