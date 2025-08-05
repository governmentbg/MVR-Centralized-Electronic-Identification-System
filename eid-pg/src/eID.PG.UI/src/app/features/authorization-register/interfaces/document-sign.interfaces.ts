import { BoricaSignatureStatus, BoricaSignatureTypeEnum, SignatureProvider } from '../enums/document-sign.enum';

export interface KEPDigestRequestData {
    digestToSign: string;
    documentName: string;
    signingCertificate: string;
    certificateChain: string[];
    encryptionAlgorithm: string;
    signingDate: Date;
}

export interface KEPSignRequestData extends KEPDigestRequestData {
    signatureValue: string;
}

export interface KEPSignResponseData {
    name: string;
    mimeType: { [key: string]: string };
    bytes: string;
    digestAlgorithm: string;
}

export interface KEPDigestDataResponseData {
    dataToSign: string;
}

export interface SignatureStatusResponseData {
    status: number;
    isProcessing: boolean;
}

export interface SignedDocumentDownloadResponseData {
    content: string;
    fileName: string;
    contentType: string;
}

export interface DocumentSignResponseData {
    threadID: string;
    groupSigning: boolean;
    transactions: {
        transactionID: string;
        identificationNumber: string;
    }[];
}

export interface BoricaDocumentSignResponseData {
    responseCode: string;
    code: string;
    message: string;
    data: {
        callbackId: string;
        validity: number;
    };
}

export interface BoricaSignRequestData {
    contents: {
        confirmText: string;
        contentFormat: string;
        mediaType: string;
        data: string;
        fileName: string;
        padesVisualSignature: boolean;
        signaturePosition: {
            imageHeight: number;
            imageWidth: number;
            imageXAxis: number;
            imageYAxis: number;
            pageNumber: number;
        };
    }[];
    relyingPartyCallbackId?: string;
    personalId?: string;
    uid: string;
}

export interface BoricaStatusResponseData {
    responseCode: BoricaSignatureStatus;
    code: string;
    message: string;
    data: {
        signatures: [
            {
                signature: string | null;
                status: BoricaSignatureStatus | null;
                signatureType: BoricaSignatureTypeEnum | null;
            }
        ];
        cert: string | null;
    };
}

export interface BoricaStatusRequestData {
    transactionId: string;
}

export interface EvrotrustSignRequestData {
    dateExpire: Date;
    documents: {
        content: string;
        fileName: string;
        contentType: string;
    }[];
    uid: string;
}

export interface EvrotrustStatusRequestData {
    transactionId: string;
    groupSigning: boolean;
}

export interface EvrotrustUserCheckResponseData {
    isRegistered: boolean;
    isIdentified: boolean;
    isRejected: boolean;
    isSupervised: boolean;
    isReadyToSign: boolean;
    hasConfirmedPhone: boolean;
    hasConfirmedEmail: boolean;
}

export interface BoricaUserCheckResponseData {
    responseCode: string;
    code: string;
    message: string;
    data: {
        certReqId: string;
        devices: string[];
        encodedCert: string;
    };
}

export interface SendSignatureRequestData {
    empowermentId: string;
    detachedSignature: string;
    signatureProvider: SignatureProvider;
}

export interface SignTransactions {
    [SignatureProvider.Evrotrust]: DocumentSignResponseData;
    [SignatureProvider.Borica]: BoricaDocumentSignResponseData;
}
