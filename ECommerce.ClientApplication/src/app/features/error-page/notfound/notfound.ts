import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-notfound',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './notfound.html',
  styleUrls: ['./notfound.scss']
})
export class Notfound {
  searchKeyword: string = '';

  constructor(private router: Router) {}

  onSearch() {
    if (this.searchKeyword && this.searchKeyword.trim().length > 0) {
      this.router.navigate(['/products'], { 
        queryParams: { keyword: this.searchKeyword } 
      });
    }
  }
}