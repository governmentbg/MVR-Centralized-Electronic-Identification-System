//
//  ApplicationStatus.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 28.02.24.
//

import SwiftUI


enum ApplicationStatus: String, Codable, CaseIterable {
    case submitted = "SUBMITTED"
    case processing = "PROCESSING"
    case pendingSignature = "PENDING_SIGNATURE"
    case signed = "SIGNED"
    case pendingPayment = "PENDING_PAYMENT"
    case paid = "PAID"
    case denied = "DENIED"
    case approved = "APPROVED"
    case generatedCertificate = "GENERATED_CERTIFICATE"
    case completed = "COMPLETED"
    case certificateStored = "CERTIFICATE_STORED"
    case paymentExpired = "PAYMENT_EXPIRED"
    case unknown = ""
}

extension ApplicationStatus {
    init(from decoder: Decoder) throws {
        let container = try decoder.singleValueContainer()
        let rawValue = try container.decode(String.self)
        self = .init(rawValue: rawValue) ?? .unknown
    }
}

extension ApplicationStatus {
    static var filterStatuses: [ApplicationStatus] {
        return [.submitted,
                .signed,
                .pendingPayment,
                .paid,
                .denied,
                .approved,
                .completed]
    }
}

extension ApplicationStatus {
    var title: String {
        switch self {
        case .submitted:
            return "application_status_submitted"
        case .processing:
            return "application_status_processing"
        case .pendingSignature:
            return "application_status_pending_signature"
        case .signed:
            return "application_status_signed"
        case .pendingPayment:
            return "application_status_pending_payment"
        case .paid:
            return "application_status_paid"
        case .denied:
            return "application_status_denied"
        case .approved:
            return "application_status_approved"
        case .generatedCertificate:
            return "application_status_generated_certificate"
        case .completed:
            return "application_status_completed"
        case .certificateStored:
            return "application_status_certificate_stored"
        case .paymentExpired:
            return "application_status_payment_expired"
        case .unknown:
            return "status_unknown"
        }
    }
    
    var iconName: String {
        switch self {
        case .submitted:
            return "icon_clock"
        case .processing:
            return "icon_clock"
        case .pendingSignature:
            return "icon_signature"
        case .signed:
            return "icon_status_active"
        case .pendingPayment:
            return "icon_clock"
        case .paid:
            return "icon_status_active"
        case .denied, .unknown:
            return "icon_forbidden"
        case .approved:
            return "icon_status_active"
        case .generatedCertificate:
            return "icon_status_active"
        case .completed:
            return "icon_status_active"
        case .certificateStored:
            return "icon_status_active"
        case .paymentExpired:
            return "icon_forbidden"
        }
    }
    
    var textColor: Color {
        switch self {
        case .submitted:
            return .buttonDanger
        case .processing:
            return .buttonDanger
        case .pendingSignature:
            return .buttonDefault
        case .signed:
            return .buttonConfirm
        case .pendingPayment:
            return .buttonDanger
        case .paid:
            return .buttonConfirm
        case .denied, .unknown:
            return .textError
        case .approved:
            return .buttonConfirm
        case .generatedCertificate:
            return .buttonConfirm
        case .completed:
            return .buttonConfirm
        case .certificateStored:
            return .buttonConfirm
        case .paymentExpired:
            return .textError
        }
    }
}
