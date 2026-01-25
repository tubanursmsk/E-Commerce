import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/authService';
import { CartService } from '../../core/services/cartService';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.scss']
})
export class Navbar {
  // HTML'de direkt erişmek için servisi public yapıyoruz
  constructor(public authService: AuthService, public cartService: CartService) {}
}
