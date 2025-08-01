export interface IEidAdministrator {
    id: string;
    name: string;
    nameLatin: string;
    eikNumber: string;
    isActive: boolean;
    contact: string;
    logo: string;
    homePage: string;
    address: string;
    administratorFrontOfficeIds: string[];
    deviceIds: string[];
}