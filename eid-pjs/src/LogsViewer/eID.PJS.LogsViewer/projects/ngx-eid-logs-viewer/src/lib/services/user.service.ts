import { Injectable } from '@angular/core';
import { IUser } from '../interfaces/IUser';
@Injectable({
    providedIn: 'root',
})
export class UserService {
    public user: IUser = {
        batchId: '',
        supplierId: '',
        userId: '',
        locale: 'bg',
        uid: '',
        uidType: '',
        name: '',
    };

    setUser(data: any) {
        this.user = {
            batchId: data.BATCHID,
            supplierId: data.SUPPLIERID,
            userId: data.USERID,
            locale: data.locale,
            uid: data.UID,
            uidType: data.UIDTYPE,
            name: data.name,
        };
    }

    getUser() {
        return this.user;
    }
}
