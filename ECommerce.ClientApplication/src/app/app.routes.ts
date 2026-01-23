
import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home'; 
import { ProductDetail } from './features/product-detail/product-detail';
import { LoginComponent } from './features/auth/login/login';


export const routes: Routes = [
    { path: '', component: HomeComponent }, // Tarayıcıda localhost:4200 açıldığında Home yüklensin
    { path: 'home', component: HomeComponent },
    { path: 'product/:id', component: ProductDetail }, // Dinamik ID ile rota
    { path: 'login', component: LoginComponent}
];