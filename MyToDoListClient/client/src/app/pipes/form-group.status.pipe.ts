import {Pipe, PipeTransform} from '@angular/core';
import {FormControl} from "@angular/forms";


/**
 * Takes a form control object and converts its status to
 * a bootstrap validation class for form groups (has-success / has-danger)
 */
@Pipe({
    name: 'formGroupStatus',
    pure: false
})
export class FormGroupStatusPipe implements PipeTransform {
    transform(control: FormControl) {
        return {
            'has-success': (control.touched && control.valid),
            'has-danger': (control.touched && control.invalid)
        };
    }
}