export interface IPaymentHistory {
    ePaymentId: string;
    citizenProfileId: string;
    createdOn: string;
    paymentDeadline: string;
    paymentDate: string;
    status: string;
    accessCode: string;
    registrationTime: string;
    referenceNumber: string;
    reason: string;
    currency: string;
    amount: number;
    lastSync: string;
}

export interface IHidePaymentHistoryEventEmitter {
    paymentHistoryEvent: IPaymentHistory[];
    refreshTable: boolean;
}
