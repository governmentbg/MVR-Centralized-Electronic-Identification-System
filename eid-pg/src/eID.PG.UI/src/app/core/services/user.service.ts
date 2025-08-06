import { Injectable } from '@angular/core';
import { IUser } from '../interfaces/IUser';
import { BehaviorSubject } from 'rxjs';
@Injectable({
    providedIn: 'root',
})
export class UserService {
    userSubject = new BehaviorSubject(false);
    public user: IUser = {
        userId: '',
        locale: '',
        uid: '',
        uidType: '',
        name: '',
        acr: '',
    };

    setUser(data: any) {
        this.user = {
            userId: data.eidenity_id,
            locale: data.locale,
            uid: data.citizen_identifier,
            uidType: data.citizen_identifier_type,
            name: [data.given_name_cyrillic, data.middle_name_cyrillic, data.family_name_cyrillic]
                .filter(s => s && s.trim())
                .join(' '),
            acr: data.acr,
        };
    }

    getUser() {
        return this.user;
    }
}
