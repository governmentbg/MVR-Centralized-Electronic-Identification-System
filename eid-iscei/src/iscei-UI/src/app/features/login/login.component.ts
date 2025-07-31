import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { IUser } from 'src/app/core/interfaces/IUser';
@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
    form!: FormGroup;
    requestInProgress!: boolean;
    user: IUser | any = null;
    originUrl = '';
    realm = '';
    client_id = '';
    provider = '';
    constructor(private route: ActivatedRoute) {}
    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.originUrl = params['origin'];
            this.realm = params['realm'];
            this.client_id = params['client_id'];
            this.provider = params['provider'];
        });
    }
}
