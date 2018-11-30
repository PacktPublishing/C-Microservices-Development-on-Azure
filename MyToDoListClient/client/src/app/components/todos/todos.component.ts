import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ToDosService } from '../../services/todos.service';
import { ToDo } from '../../models/todo.model';
import { ToDosViewModel } from '../../view-models/todos.view-model';

@Component({
    selector: 'todos',
    templateUrl: 'todos.component.html'
})

export class ToDosComponent implements OnInit {
    todos: ToDo[] = [];
    
    constructor(public todosVm: ToDosViewModel) {}

    ngOnInit() { 
        this.todosVm.initialize();
    } 
}