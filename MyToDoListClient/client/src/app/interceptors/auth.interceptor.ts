import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { map, flatMap } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private _allowedUrls: string[] = ['api/users/signup', 'api/users/signIn']
    
    private isAllowedRoute(url: string) {
        return this._allowedUrls.reduce((acc, route) => {
            return acc || url.indexOf(route) >= 0
        }, false);
    }

    constructor(private _authService: AuthService) {

    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (this.isAllowedRoute(req.url)) {
            return next.handle(req);
        }
        const accessToken = this._authService.accessToken;
        const headers = req.headers
            .set('Content-Type', 'application/json')
            .set('Authorization',`Bearer ${accessToken}`)
        const authReq = req.clone({ headers });
        return next.handle(authReq);
    }
}