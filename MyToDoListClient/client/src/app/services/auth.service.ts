import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../environments/environment";
import { User } from "../models/user.model";
import { Observable, Subscriber, Observer } from 'rxjs';
import * as jwt_decode from 'jwt-decode';

@Injectable()
export class AuthService {
    private _baseUrl: string = environment.serverRoot;
    private _endpoints = {
        signUp: `${this._baseUrl}${environment.endpoints.auth.signUp}`,
        signIn: `${this._baseUrl}${environment.endpoints.auth.signIn}`
    };
    private _accessTokenObserver: Observer<string>;
    public accessToken$: Observable<string>;

    get accessToken(): string {
        return localStorage.getItem("accessToken");
    }

    set accessToken(value) {
        if(!value) {
            localStorage.removeItem("accessToken");
        }
        else{
            localStorage.setItem("accessToken", value);
            const decoded = jwt_decode(value);
            if(decoded) {
                localStorage.setItem("user", JSON.stringify({firstName: decoded.FirstName, lastName: decoded.LastName}) );
            }
        }
        this._accessTokenObserver.next(value);
    }

    get user(): User {
        let parsed = localStorage.getItem("user");
        if (parsed) {
            return JSON.parse(parsed) as User;
        }
        else {
            return {
                firstName: "",
                lastName: ""
            } as User;
        }
    }

    constructor(private _http: HttpClient) {
        this.accessToken$ = Observable.create(observer => {
            this._accessTokenObserver = observer;
            this._accessTokenObserver.next(this.accessToken);
        });
        this.accessToken$.subscribe();
    }

    private authOperation(url: string, user: User): Promise<string> {
        return this._http.post(url, user, {responseType: 'text'}).toPromise().then(token => {
            this.accessToken = token;
            return token;
        });
    }

    signUp(user: User): Promise<string> {
        return this.authOperation(this._endpoints.signUp, user);        
    }

    signIn(user: User): Promise<string> {
        return this.authOperation(this._endpoints.signIn, user);
    }

    logout(): Promise<void> {
        this.accessToken = "";
        return Promise.resolve();
    }
}
