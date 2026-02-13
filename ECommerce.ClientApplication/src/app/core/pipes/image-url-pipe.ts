import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'imageUrl',
  standalone: true
})
export class ImageUrlPipe implements PipeTransform {

  // Backend adresin (RestApi launchSettings.json'daki https portu)
  private apiUrl = 'http://localhost:5271'; 

  transform(value: string | undefined | null): string {
    // 1. Resim yoksa placeholder göster
    if (!value) {
      return 'assets/placeholder.png'; // assets klasörüne bir tane placeholder.png atabiliriz
    }

    // 2. Eğer resim zaten tam bir URL ise (Amazon, Vatan linkleri gibi) olduğu gibi döndür
    if (value.startsWith('http') || value.startsWith('https')) {
      return value;
    }

    // 3. Eğer resim relative path ise (/images/...) başına API adresini ekle
    return `${this.apiUrl}${value}`;
  }

}