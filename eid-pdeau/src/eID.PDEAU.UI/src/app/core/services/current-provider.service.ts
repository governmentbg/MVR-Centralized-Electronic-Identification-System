import { Injectable } from '@angular/core';
import { detailedProviderSchema, DetailedProviderType } from '@app/features/providers/provider.dto';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class CurrentProviderService {
    constructor(private http: HttpClient) {}
    private provider: DetailedProviderType | null = null;
    public providerIsLoading = false;

    async init(cb?: () => void) {
        this.providerIsLoading = true;
        this.fetchCurrentProvider().subscribe({
            next: provider => {
                this.provider = provider;
                this.providerIsLoading = false;
                if (cb) {
                    cb();
                }
            },
            error: () => {
                this.providerIsLoading = false;
                throw new Error('Error loading Provider');
            },
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
