export interface ILog {
    eventDate: string;
    eventId: string;
    eventType: string;
    requesterSystemId: string;
    requesterSystemName: string;
}

export interface IGetLogs {
    startDate?: string;
    endDate?: string;
    eventTypes?: string[];
    cursorSize: number;
    cursorSearchAfter: string[];
}
