import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { PaginationData, SortData } from '@app/shared/types';
import { createHttpParams } from '@app/shared/utils/create-http-params';
import { createPaginatedResponseSchema } from '@app/shared/schemas';
import { CreateUserType, getUserSchema } from './users.dto';

@Injectable({
    providedIn: 'root',
})
export class UsersService {
    private apiUrl = '/admin/api/v1/Providers/users';

    constructor(private http: HttpClient) {}

    fetchUsers = (
        payload: {
            [key: string]: any;
        } & PaginationData &
            SortData
    ) => {
        const queryParams = createHttpParams(payload);
        return this.http
            .get(this.apiUrl, { params: queryParams })
            .pipe(map(data => createPaginatedResponseSchema(getUserSchema).parse(data)));
    };

    registerUser = (data: CreateUserType) => {
        return this.http.post(this.apiUrl, data);
    };

    editUser = (id: string | undefined, data: CreateUserType) => {
        return this.http.put(`${this.apiUrl}/${id}`, data);
    };

    getUserById = (id: string) => {
        return this.http.get(`${this.apiUrl}/${id}`).pipe(map(data => getUserSchema.parse(data)));
    };

    deleteUser = (id: string) => {
        return this.http.delete(`${this.apiUrl}/${id}`);
    };
}
