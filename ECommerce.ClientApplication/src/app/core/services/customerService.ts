import { Injectable } from '@angular/core';
import { BaseService } from './baseService';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../models/apiResponse';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private baseService: BaseService) { }

  // Profil Bilgilerini Getir
  getProfile(): Observable<any> {
    return this.baseService.get<ApiResponse<any>>('Customer/GetProfile').pipe(
      map(res => res.data)
    );
  }

  // Profil Güncelle
  updateProfile(data: any): Observable<ApiResponse<boolean>> {
    return this.baseService.put<ApiResponse<boolean>>('Customer/UpdateProfile', data);
  }
}