export interface Review {
    id: string;
    comment: string;
    rating: number; // 1-5 arası
    customerName: string; // Backend DTO'da bu alanı maplediğini varsayıyorum
    createdDate: string;
}