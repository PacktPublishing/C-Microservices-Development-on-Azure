import { Component, OnInit } from '@angular/core';
import { ToDosViewModel } from '../../../view-models/todos.view-model';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { ToDo } from '../../../models/todo.model';

@Component({
    selector: 'create-todo',
    templateUrl: 'create-todo.component.html',
    styleUrls: ['./create-todo.component.scss']
})
export class CreateToDoComponent {
    editMode: boolean = false;
    todoForm: FormGroup = new FormGroup({
        'title': new FormControl("", [Validators.required, Validators.maxLength(255)]),
        'body': new FormControl("", [Validators.required])
    });

    get titleInput(): AbstractControl {
        return this.todoForm.get('title');
    }

    get bodyInput(): AbstractControl {
        return this.todoForm.get('body');
    }

    constructor(public todosVm: ToDosViewModel) { }
    
    toggleEditMode() {
        this.editMode = !this.editMode;
    }

    save() {
        if(this.todoForm.valid) {
            this.todosVm.newTodo.title = this.titleInput.value;
            this.todosVm.newTodo.body = this.bodyInput.value;
            this.todosVm.newTodo.isCompleted = false;
            this.todosVm.create().then(() => {
                this.titleInput.setValue("");
                this.bodyInput.setValue("");
                this.editMode = false;
            });
        }
    }
}