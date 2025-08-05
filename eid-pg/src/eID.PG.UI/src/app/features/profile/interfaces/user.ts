export interface User {
    id: string;
    active: boolean;
    citizenIdentifierNumber: string;
    citizenIdentifierType: string;
    citizenProfileId: string;
    firstName: string;
    secondName: string;
    lastName: string;
    firstNameLatin: string;
    secondNameLatin: string;
    lastNameLatin: string;
    eidentityId: string;
    phoneNumber: string;
    email: string;
    is2FaEnabled: boolean;
}
