import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { UserType } from '@app/features/providers/provider.dto';
import { Table } from 'primeng/table';

@Component({
    selector: 'app-users-table',
    templateUrl: './users-table.component.html',
    styleUrls: ['./users-table.component.scss'],
})
export class UsersTableComponent {
    @ViewChild('usersTable') table!: Table;
    @ViewChild('filterInput') filterInput!: ElementRef<HTMLInputElement>;
    @Input() users: UserType[] = [];

    globalFilter(event: Event) {
        this.table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
    }

    clearGlobalFilter() {
        this.table.clear();
        this.filterInput.nativeElement.value = '';
    }
}
