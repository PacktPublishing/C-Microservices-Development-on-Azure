import {Component} from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { User } from '../../../models/user.model';
import { Router } from '@angular/router';

@Component({
    selector: 'sign-in',
    templateUrl: './sign-incomponent.html',
    styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent {
    accessDenied: boolean = false;

    constructor(private _authService: AuthService, private _router: Router) {

    }

    signInForm: FormGroup = new FormGroup({
        'email': new FormControl("", [Validators.required, Validators.email]),
        'password': new FormControl("", [Validators.required])
    });

    get emailInput(): AbstractControl {
        return this.signInForm.get('email');
    }
    
    get passwordInput(): AbstractControl {
        return this.signInForm.get('password');
    }

    signIn(): Promise<void> {
        this.accessDenied = false;
        if(this.signInForm.valid) {
            let user: User = {
                firstName: "",
                lastName: "",
                email: this.emailInput.value,
                password: this.passwordInput.value
            }
            return this._authService.signIn(user).then((token) => {
                this._router.navigate(['home']);
            }).catch(err => {
                this.accessDenied = true;
            })
        }
    }
}