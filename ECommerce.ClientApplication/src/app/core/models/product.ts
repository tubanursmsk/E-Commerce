export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  discountPrice?: number;
  isFreeShipping: boolean;
  isFastDelivery: boolean;
  imageUrl: string;
  stock: number;
  categoryId: string;
  companyId: string;
  categoryName?: string;
  brandName?: string;
}

// Sol paneldeki Marka filtreleri için (Örn: Apple (15))
export interface BrandFilter {
  id: string;
  name: string;
  count: number;
}

// Backend'den dönen "Paket" veri yapısı
export interface ProductListResponse {
  products: Product[];
  totalCount: number;
  availableBrands: BrandFilter[];
  minPrice: number;
  maxPrice: number;
}

// YENİ: Filtreleme parametreleri (İsteği atarken kullanacağız)
export interface ProductFilterParams {
  categoryId?: string;
  brandIds?: string[]; // Çoklu marka seçimi için dizi
  minPrice?: number;
  maxPrice?: number;
  keyword?: string;
  isFreeShipping?: boolean;
  isFastDelivery?: boolean;
  sortBy?: string;     // 'price_asc', 'price_desc', 'newest'
  pageNumber?: number;
  pageSize?: number;
}