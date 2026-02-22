import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-servererror',
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './servererror.html',
  styleUrl: './servererror.scss',
})
export class Servererror {

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