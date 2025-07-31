export interface IChannel {
    id: string;
    systemId: string;
    name: string;
    description: string;
    callbackUrl: string;
    price: number;
    infoUrl: string;
    email: string;
    translations: { language: string; name: string; description: string }[];
    reason?: string;
    modifiedOn: string;
}

export interface IChannelResponseData {
    pending: IChannel[];
    approved: IChannel[];
    rejected: IChannel[];
    archived: IChannel[];
}

export interface IChannelTestResponseData {
    isSuccess: boolean;
    statusCode: string;
    response: string;
}
