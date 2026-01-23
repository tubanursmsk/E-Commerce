import { Injectable } from '@angular/core';
import { BaseService } from './baseService';
import { Observable, map } from 'rxjs';
import { Review } from '../models/review';
import { ApiResponse } from '../models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {

  constructor(private baseService: BaseService) { }

  // Belirli bir ürünün yorumlarını getir
  getReviewsByProductId(productId: string): Observable<Review[]> {
    return this.baseService.get<ApiResponse<Review[]>>(`Review/Product/${productId}`).pipe(
      map(response => response.data)
    );
  }
}