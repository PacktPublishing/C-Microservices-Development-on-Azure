import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';

@Injectable()
export class AuthGuard implements CanActivate {
    publicRoutes: string[] = ['', 'sign-up'];
    
    constructor(private _authService: AuthService, private _router: Router) {

    }
    
    inPublicRoute(path: string): boolean {
        return this.publicRoutes.indexOf(path) >= 0;
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
        return this._authService.accessToken$.pipe(map(accessToken => {
            if(accessToken) {
                if(this.inPublicRoute(route.routeConfig.path)) {
                    this._router.navigate(['home']);
                }
                return true;
            } else {
                if(!this.inPublicRoute(route.routeConfig.path)) {
                    this._router.navigate(['']);
                    return false;
                }
                else {
                    return true;
                }
            }
        }));
    }
}