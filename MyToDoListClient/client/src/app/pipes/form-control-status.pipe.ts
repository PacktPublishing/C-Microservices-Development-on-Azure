import {Pipe, PipeTransform} from '@angular/core';
import {FormControl} from "@angular/forms";

@Pipe({
    name: 'formControlStatus',
    pure: false
})
export class FormControlStatusPipe implements PipeTransform {
    transform(control: FormControl): any {
        return {
            'form-control-success': (control.touched && control.valid),
            'form-control-danger': (control.touched && control.invalid)
        };
    }
}