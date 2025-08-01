import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ServicesTariffsComponent } from '@app/features/tariffs/pages/services-tariffs/services-tariffs.component';
import { ServicesTariffsFormComponent } from '@app/features/tariffs/components/services-tariffs-form/services-tariffs-form.component';
import { DiscountsComponent } from '@app/features/tariffs/pages/discounts/discounts.component';
import { DiscountFormComponent } from '@app/features/tariffs/components/discount-form/discount-form.component';

const routes: Routes = [
    {
        path: '',
        pathMatch: 'full',
        redirectTo: 'services',
    },
    {
        path: 'services',
        component: ServicesTariffsComponent,
    },
    {
        path: 'services/new',
        component: ServicesTariffsFormComponent,
    },
    {
        path: 'services/edit',
        component: ServicesTariffsFormComponent,
    },
    {
        path: 'discounts',
        component: DiscountsComponent,
    },
    {
        path: 'discounts/new',
        component: DiscountFormComponent,
    },
    {
        path: 'discounts/edit',
        component: DiscountFormComponent,
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class TariffsRoutingModule {}
