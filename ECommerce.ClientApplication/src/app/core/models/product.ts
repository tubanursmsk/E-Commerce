export interface Product {
  companyId: any;
  id: string;
  name: string;
  description: string;
  price: number;
  discountPrice?: number; // İndirimli fiyat varsa
  isFreeShipping: boolean; 
  isFastDelivery: boolean;
  imageUrl: string;
  stock: number;
  categoryId: string;
  categoryName?: string;
  brandName?: string;
}