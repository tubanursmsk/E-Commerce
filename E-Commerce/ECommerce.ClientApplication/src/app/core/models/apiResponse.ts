export interface ApiResponse<T> {
  data: T;           // API'den dönen asıl veri (Banner listesi, Ürün vb.)
  success: boolean;  // İşlem başarılı mı?
  message: string;   // Hata veya başarı mesajı
  code: number;      // HTTP durum kodu (200, 400, 404 vb.)
  timestamp: string; // İşlem zamanı
}

//!apiResponse.ts dosya kurulum amacı:
/*.NET API tarafında yazdığımız ApiResponse<T> sınıfının Angular (TypeScript) tarafında da bir 
 karşılığı olması gerekiyor ki servislerin gelen veriyi (Data, Success, Message, Code) doğru tanıyabilsin.

//*Neden Buna İhtiyacımız Var?
 API tarafında veriyi gönderirken direkt listeyi değil, bir paket (wrapper) içinde gönderiyorsun.

API Çıktın: { "data": [...], "success": true, "message": "..." } şeklinde.

Angular: Eğer ApiResponse modelini kullanmazsak, Angular gelen bu paketin içindeki data kısmına nasıl 
ulaşacağını bilemez. map(response => response.data) dediğimizde, TypeScript'e "Bu gelen bir pakettir ve 
içindeki data kısmını bana ver" demiş oluyorux.*/