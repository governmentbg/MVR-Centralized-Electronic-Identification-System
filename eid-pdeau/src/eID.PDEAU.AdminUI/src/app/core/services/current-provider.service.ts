import { Injectable } from '@angular/core';
import { detailedProviderSchema, DetailedProviderType } from '@app/features/providers/provider.dto';
import { HttpClient } from '@angular/common/http';
import { UserService } from './user.service';
import { map } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class CurrentProviderService {
    constructor(private http: HttpClient, private userService: UserService) {}
    private provider: DetailedProviderType | null = null;

    async init(cb: () => void) {
        this.fetchCurrentProvider().subscribe(provider => {
            this.provider = provider;
            cb();
        });
    }

    getProvider() {
        return this.provider;
    }

    fetchCurrentProvider() {
        return this.http.get('/admin/api/v1/Providers/current').pipe(map(data => detailedProviderSchema.parse(data)));
    }

    isOfType(type: DetailedProviderType['type']) {
        return this.provider?.type === type;
    }
}
