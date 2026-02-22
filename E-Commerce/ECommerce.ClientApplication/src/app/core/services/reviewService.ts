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
  // Yorum Kaydet
  createReview(reviewData: any): Observable<ApiResponse<string>> {
    return this.baseService.post<ApiResponse<string>>('Review/Create', reviewData);
  }

  updateReview(id: string, reviewData: any): Observable<ApiResponse<boolean>> {
    return this.baseService.post<ApiResponse<boolean>>(`Review/Update/${id}`, reviewData);
  }

  deleteReview(id: string): Observable<ApiResponse<boolean>> {
    return this.baseService.delete<ApiResponse<boolean>>(`Review/Delete/${id}`);
  }
}
