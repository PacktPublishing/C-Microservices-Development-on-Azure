import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ToDo } from '../models/todo.model';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ToDosService {
    private _baseUrl: string = environment.serverRoot;
    private _endpoint: string = `${this._baseUrl}${environment.endpoints.todos}`

    constructor(private _httpClient: HttpClient) {
    }

    getAll(): Promise<ToDo[]> {
        return this._httpClient.get<ToDo[]>(this._endpoint).toPromise();
    }

    create(todo: ToDo): Promise<ToDo> {
        return this._httpClient.post<ToDo>(this._endpoint, todo).toPromise();
    }

    update(id: number, todo: ToDo): Promise<ToDo> {
        return this._httpClient.put<ToDo>(`${this._endpoint}/${id}`, todo).toPromise();
    }

    updateStatus(id: number, status: boolean): Promise<ToDo> {
        return this._httpClient.patch<ToDo>(`${this._endpoint}/${id}`, {isCompleted: status}).toPromise();
    }

    delete(id: number): Promise<any> {
        return this._httpClient.delete<any>(`${this._endpoint}/${id}`).toPromise();
    }
}