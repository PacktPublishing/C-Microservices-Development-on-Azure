import { User } from "./user.model";

export interface ToDo {
    id?: number;
    title: string;
    body: string;
    isCompleted: boolean;
    createdAt?: Date;
    updatedAt?: Date;
    userId?: number;
    user?: User;
}