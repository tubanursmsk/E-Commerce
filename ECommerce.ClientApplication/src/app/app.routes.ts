import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home'; 
export const routes: Routes = [
    { path: '', component: HomeComponent }, // Tarayıcıda localhost:4200 açıldığında Home yüklensin
    { path: 'home', component: HomeComponent }
];