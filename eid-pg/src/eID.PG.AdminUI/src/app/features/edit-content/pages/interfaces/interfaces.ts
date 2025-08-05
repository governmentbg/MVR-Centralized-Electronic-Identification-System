export interface IHideEditPageEmitter {
    editContent: boolean;
    updateContent: boolean;
}

export interface IBreadCrumbItems {
    label?: string;
    routerLink?: string;
    onClick?: () => any;
}
