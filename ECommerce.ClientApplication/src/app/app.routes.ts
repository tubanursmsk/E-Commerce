import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home'; 
import { ProductDetail } from './features/product-detail/product-detail';
import { LoginComponent } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { Favorites } from './features/favorites/favorites';
import { Cart } from './features/cart/cart';
import { Profile } from './features/profile/profile';


export const routes: Routes = [
    { path: '', component: HomeComponent }, // Tarayıcıda localhost:4200 açıldığında Home yüklensin
    { path: 'home', component: HomeComponent },
    { path: 'product/:id', component: ProductDetail }, // Dinamik ID ile rota
    { path: 'login', component: LoginComponent},
    { path: 'register', component: Register},
    { path: 'cart', component: Cart },
    { path: 'favorites', component: Favorites },
    { path: 'profile', component: Profile },
];