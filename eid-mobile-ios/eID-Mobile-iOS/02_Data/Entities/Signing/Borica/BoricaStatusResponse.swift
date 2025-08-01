//
//  BoricaStatusResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


struct BoricaStatusResponse: Codable {
    // MARK: - Properties
    var responseCode: BoricaStatusResponseCode
    var code: String
    var message: String
    var data: BoricaSignatureStatusResponse
}

struct BoricaSignatureStatusResponse: Codable {
    // MARK: - Properties
    var signatures: [BoricaSignature]
    var cert: String?
}

struct BoricaSignature: Codable {
    // MARK: - Properties
    var signature: String?
    var signatureType: BoricaSignatureType?
    var status: BoricaSignatureStatus
}

enum BoricaStatusResponseCode: String, Codable {
    case accepted = "ACCEPTED"
    case inProgress = "IN_PROGRESS"
    case rejected = "REJECTED"
    case completed = "COMPLETED"
}

enum BoricaSignatureType: String, Codable {
    case CADES_BASELINE_B_DETACHED = "CADES_BASELINE_B_DETACHED"
    case PADES_BASELINE_B = "PADES_BASELINE_B"
    case XADES_BASELINE_B_ENVELOPED = "XADES_BASELINE_B_ENVELOPED"
}

enum BoricaSignatureStatus: String, Codable {
    case error = "ERROR"
    case inProgress = "IN_PROGRESS"
    case signed = "SIGNED"
    case received = "RECEIVED"
    case rejected = "REJECTED"
    case archived = "ARCHIVED"
    case removed = "REMOVED"
    case expired = "EXPIRED"
}
