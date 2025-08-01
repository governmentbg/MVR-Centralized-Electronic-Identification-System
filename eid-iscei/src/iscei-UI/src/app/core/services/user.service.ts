import { Injectable } from '@angular/core';
import { IUser } from '../interfaces/IUser';
import { BehaviorSubject } from 'rxjs';
@Injectable({
    providedIn: 'root',
})
export class UserService {
    userSubject = new BehaviorSubject(false);
    public user: IUser = {
        providerDetailsId: '',
        providerId: '',
        userId: '',
        locale: '',
        uid: '',
        uidType: '',
        name: '',
    };

    setUser(data: any) {
        this.user = {
            providerDetailsId: data.PROVIDERID,
            providerId: data.PROVIDERID,
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
