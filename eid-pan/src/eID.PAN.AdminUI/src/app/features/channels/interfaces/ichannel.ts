export interface IChannel {
    id: string;
    systemId: string;
    name: string;
    description: string;
    callbackUrl: string;
    price: number;
    infoUrl: string;
    translations: { language: string; name: string; description: string }[];
}

export interface IChannelResponseData {
    pending: IChannel[];
    approved: IChannel[];
    rejected: IChannel[];
    archived: IChannel[];
}
