import { Component, OnInit } from '@angular/core';
import { ToDosViewModel } from '../../../view-models/todos.view-model';
import { ToDo } from '../../../models/todo.model';

@Component({
    selector: 'todos-list',
    templateUrl: 'todos-list.component.html',
    styleUrls: ['./todos-list.component.scss']
})

export class ToDosListComponent {
    constructor(public todosVm: ToDosViewModel) {

    }

    updateToDoStatus(todo: ToDo) {
        this.todosVm.setUpdatingToDo(todo);
        this.todosVm.updatingTodo.isCompleted = !this.todosVm.updatingTodo.isCompleted;
        this.todosVm.updateStatus();
    }
}