import {Component} from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { User } from '../../../models/user.model';
import { Router } from '@angular/router';

@Component({
    selector: 'sign-up',
    templateUrl: './sign-up.component.html',
    styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent {
    signUpForm: FormGroup = new FormGroup({
        'firstName': new FormControl("", [Validators.required, Validators.minLength(2), Validators.maxLength(32)]),
        'lastName': new FormControl("", [Validators.required, Validators.minLength(2), Validators.maxLength(32)]),
        'email': new FormControl("", [Validators.required, Validators.email]),
        'password': new FormControl("", [Validators.required, Validators.minLength(8)])
    });
    loading: boolean = false;
    signUpFailed: boolean = false;
    reqError: string = "";

    get firstNameInput(): AbstractControl {
        return this.signUpForm.get('firstName');
    }

    get lastNameInput(): AbstractControl {
        return this.signUpForm.get('lastName');
    }

    get emailInput(): AbstractControl {
        return this.signUpForm.get('email');
    }

    get passwordInput(): AbstractControl {  
        return this.signUpForm.get('password');
    }

    get errorMessage(): string {
        let msg: string = "";
        if(this.reqError) {
            msg = this.reqError;
        }
        else if(this.firstNameInput.errors && this.firstNameInput.errors.required) {
            msg = "First Name is a required field";
        }
        else if (this.firstNameInput.errors && this.firstNameInput.errors.minlength) {
            msg = "First Name is too short";
        }
        else if (this.firstNameInput.errors && this.firstNameInput.errors.maxlength) {
            msg = "First Name is too long";
        }
        else if(this.lastNameInput.errors && this.lastNameInput.errors.required) {
            msg = "Last Name is a required field";
        }
        else if(this.lastNameInput.errors && this.lastNameInput.errors.minlength) {
            msg = "Last Name is too short";
        }
        else if(this.lastNameInput.errors && this.lastNameInput.errors.maxlength) {
            msg = "Last Name is too long";
        }
        else if(this.emailInput.errors && this.emailInput.errors.required) {
            msg = "Email is a required field"
        }
        else if(this.emailInput.errors && this.emailInput.errors.email) {
            msg = "Email is invalid"
        }
        else if(this.passwordInput.errors && this.passwordInput.errors.required) {
            msg = "Password is a required field";
        }
        else if(this.passwordInput.errors && this.passwordInput.errors.minlength) {
            msg = "Password is too short";
        }
        return msg; 
    }

    constructor(private _authService: AuthService, private _router: Router) {
    }

    signUp(): Promise<void> {
        this.loading = true;
        this.signUpFailed = false;
        this.reqError = "";

        if(this.signUpForm.valid) {
            let user: User = {
                firstName: this.firstNameInput.value,
                lastName: this.lastNameInput.value,
                email: this.emailInput.value,
                password: this.passwordInput.value,
            };
            return this._authService.signUp(user).then(() => {
                this.loading = false;
                this._router.navigate(["home"]);
            }).catch(err => {
                if(err.status == 400) {
                    this.reqError = err.error.indexOf("already exists") >= 0 ? err.error : "Oops something went wrong"
                }
                else {
                    this.reqError = "Oops! Something went wrong";
                }
                this.signUpFailed = true;
            });
        }
    }
}