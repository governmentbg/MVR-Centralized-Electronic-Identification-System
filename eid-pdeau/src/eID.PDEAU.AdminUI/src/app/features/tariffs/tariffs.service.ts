import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { DiscountListSchema, ProvidedServiceListSchema, TariffListSchema } from '@app/features/tariffs/tariffs.dto';
import { UserService } from '@app/core/services/user.service';

const baseApiUrl = '/raeicei/api/v1';
const circumstancesApiUrl = '/raeicei/api/v1/circs';

@Injectable({
    providedIn: 'root',
})
export class TariffsService {
    private apiUrl = baseApiUrl;
    private circumstancesApiUrl = circumstancesApiUrl;

    constructor(private http: HttpClient, private userService: UserService) {}

    fetchTariffs = () => {
        return this.http
            .get(`${this.apiUrl}/tariff/getAll/${this.userService.user.eidadministratorid}`)
            .pipe(map(data => TariffListSchema.parse(data)));
    };

    createTariff = (payload: any) => {
        return this.http.post(`${this.circumstancesApiUrl}/tariff/create`, payload);
    };

    updateTariff = (payload: any, id: string) => {
        return this.http.put(`${this.circumstancesApiUrl}/tariff/update/${id}`, payload);
    };

    deleteTariff = (id: string) => {
        return this.http.delete(`${this.circumstancesApiUrl}/tariff/delete/${id}`);
    };

    fetchProvidedServices = () => {
        return this.http
            .get(`${this.apiUrl}/providedservice/getAll`)
            .pipe(map(data => ProvidedServiceListSchema.parse(data)));
    };

    fetchDiscounts = () => {
        return this.http
            .get(`${this.apiUrl}/discount/getAll/${this.userService.user.eidadministratorid}`)
            .pipe(map(data => DiscountListSchema.parse(data)));
    };

    createDiscount = (payload: any) => {
        return this.http.post(`${this.circumstancesApiUrl}/discount/create`, payload);
    };

    updateDiscount = (payload: any, id: string) => {
        return this.http.put(`${this.circumstancesApiUrl}/discount/update/${id}`, payload);
    };

    deleteDiscount = (id: string) => {
        return this.http.delete(`${this.circumstancesApiUrl}/discount/delete/${id}`);
    };
}
