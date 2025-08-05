import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
@Injectable({
    providedIn: 'root',
})
export class UsefulInformationService {
    informationTabSubject = new BehaviorSubject(false);
}
