import { ImageUrlPipe } from './image-url-pipe';

describe('ImageUrlPipe', () => {
  it('create an instance', () => {
    const pipe = new ImageUrlPipe();
    expect(pipe).toBeTruthy();
  });
});

/*

Veritabanına kaydettiğimiz veri şu şekilde: /images/products/resimadı.jpg (Bu bir Relative Path yani Göreceli Yol).
Sorun Şu: Angular (Client) tarafında bu veriyi src içine koyduğunda, tarayıcı bunu Angular'ın çalıştığı 
adreste arıyor: http://localhost:4200/images/products/... -> Böyle bir yer yok!
Resimler aslında Backend sunucusunda: https://localhost:5271/images/products/...
Hem eski Seed Data (Amazon linkleri - https://...) hem de yeni yüklenen Local Data (/images/...)
 ile uyumlu çalışacak profesyonel bir çözüm uygulayalım: Angular Pipe.
Çözüm: ImageUrlPipe Oluşturma
Bir "Pipe" yazacağız. Bu araç, resim yoluna bakacak; eğer başında http varsa dokunmayacak, yoksa başına 
Backend adresini ekleyecek.
1. Adım: Pipe Oluşturma
Terminalde şu komutu çalıştır: ng generate pipe core/pipes/imageUrl

*/