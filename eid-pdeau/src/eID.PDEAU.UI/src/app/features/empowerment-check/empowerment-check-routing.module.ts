import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { EmpowermentCheckSearchComponent } from './pages/empowerment-check-search/empowerment-check-search.component';
import { EmpowermentCheckSearchResultsComponent } from './pages/empowerment-check-search-results/empowerment-check-search-results.component';
import { EmpowermentCheckSearchResultDetailsComponent } from './pages/empowerment-check-search-result-details/empowerment-check-search-result-details.component';
import { RoleType } from '@app/core/enums/auth.enum';

const routes: Routes = [
    {
        path: '',
        component: EmpowermentCheckSearchComponent,
        canActivate: [AuthGuard],
    },
    {
        path: 'search-results',
        component: EmpowermentCheckSearchResultsComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.EMPLOYEE, RoleType.ADMINISTRATOR] },
    },
    {
        path: 'search-result-details',
        component: EmpowermentCheckSearchResultDetailsComponent,
        canActivate: [AuthGuard],
        data: { roles: [RoleType.EMPLOYEE, RoleType.ADMINISTRATOR] },
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class EmpowermentCheckRoutingModule {}
