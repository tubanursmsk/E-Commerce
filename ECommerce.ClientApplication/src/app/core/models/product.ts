export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  discountPrice?: number; // İndirimli fiyat varsa
  imageUrl: string;
  stock: number;
  categoryId: string;
  categoryName?: string;
  brandName?: string;
}