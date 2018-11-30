import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { AuthService } from './services/auth.service';
import { SignUpComponent } from './components/auth/sign-up/sign-up.component';
import { FormControlStatusPipe } from './pipes/form-control-status.pipe';
import { FormGroupStatusPipe } from './pipes/form-group.status.pipe';
import { SignInComponent } from './components/auth/sign-in/sign-in.component';
import { WelcomeComponent } from './components/welcome/welcome.component';
import { HomeComponent } from './components/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { SignUpPageComponent } from './components/sign-up-page/sign-up-page.component';
import { ToDosComponent } from './components/todos/todos.component';
import { ToDosService } from './services/todos.service';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ToDosListComponent } from './components/todos/todos-list/todos-list.component';
import { CreateToDoComponent } from './components/todos/create-todo/create-todo.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { ToDosViewModel } from './view-models/todos.view-model';

const routes: Routes = [
  { path: '', component: WelcomeComponent, canActivate: [AuthGuard] },
  { path: 'sign-up', component: SignUpPageComponent, canActivate: [AuthGuard] },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
]

@NgModule({
  declarations: [
    AppComponent,
    SignUpComponent,
    SignInComponent,
    FormControlStatusPipe,
    FormGroupStatusPipe,
    WelcomeComponent,
    HomeComponent,
    SignUpPageComponent,
    ToDosComponent,
    ToDosListComponent,
    CreateToDoComponent,
    NavbarComponent,
    CreateToDoComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(routes),
  ],
  providers: [AuthService, AuthGuard, ToDosService, {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true}, ToDosViewModel],
  bootstrap: [AppComponent]
})
export class AppModule { }
