import { Injectable } from "@angular/core";
import { ToDo } from "../models/todo.model";
import { ToDosService } from "../services/todos.service";

@Injectable()
export class ToDosViewModel {
    loading: boolean = false;
    todos: ToDo[] = [];
    newTodo: ToDo = {} as ToDo;
    updatingTodo: ToDo = {} as ToDo;

    constructor(private _todosService: ToDosService) {
        
    }

    private clearNewToDo() {
        this.newTodo = {} as ToDo;
    }

    private clearUpdatingToDo() {
        this.updatingTodo = {} as ToDo;
    }

    private clearAll() {
        this.clearNewToDo();
        this.clearUpdatingToDo();
    }

    initialize(): Promise<void> {
        return this.refresh();
    }

    setUpdatingToDo(todo: ToDo) {
        this.updatingTodo = todo;
    }

    refresh(): Promise<void> {
        this.loading = true;
        return this._todosService.getAll().then(todos => {
            this.todos = todos;
            this.loading = false;
            this.clearAll();
        });
    }

    create(): Promise<void> {
        this.loading = true;
        return this._todosService.create(this.newTodo).then(() => this.refresh());
    }

    update(): Promise<void> {
        this.loading = true;
        return this._todosService.update(this.updatingTodo.id, this.updatingTodo).then(() => this.refresh());
    }

    updateStatus(): Promise<void> {
        this.loading = true;
        return this._todosService.updateStatus(this.updatingTodo.id, this.updatingTodo.isCompleted).then(() => this.refresh());
    }

    delete(id: number): Promise<void> {
        this.loading = true;
        return this._todosService.delete(id).then(() => this.refresh());
    }
}