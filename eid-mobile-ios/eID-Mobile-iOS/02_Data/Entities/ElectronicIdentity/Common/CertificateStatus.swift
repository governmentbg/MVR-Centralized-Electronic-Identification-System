//
//  CertificateStatus.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.02.24.
//

import SwiftUI


enum CertificateStatus: String, Codable, CaseIterable {
    case created = "CREATED"
    case signed = "SIGNED"
    case invalid = "INVALID"
    case active = "ACTIVE"
    case stopped = "STOPPED"
    case revoked = "REVOKED"
    case failed = "FAILED"
    case expired = "EXPIRED"
    case unknown = ""
}

extension CertificateStatus {
    init(from decoder: Decoder) throws {
        let container = try decoder.singleValueContainer()
        let rawValue = try container.decode(String.self)
        self = .init(rawValue: rawValue) ?? .unknown
    }
}

extension CertificateStatus {
    static var filterStatuses: [CertificateStatus] {
        return [.active,
                .stopped,
                .revoked,
                .expired]
    }
}

extension CertificateStatus {
    var title: String {
        switch self {
        case .created:
            return "certificate_status_created"
        case .signed:
            return "certificate_status_signed"
        case .invalid:
            return "certificate_status_invalid"
        case .active:
            return "certificate_status_active"
        case .stopped:
            return "certificate_status_stopped"
        case .revoked:
            return "certificate_status_revoked"
        case .failed:
            return "certificate_status_failed"
        case .expired:
            return "certificate_status_expired"
        case .unknown:
            return "status_unknown"
        }
    }
    
    var iconName: String {
        switch self {
        case .created:
            return "icon_clock"
        case .signed:
            return "icon_signature"
        case .invalid,
                .expired,
                .unknown:
            return "icon_expired"
        case .active:
            return "icon_status_active"
        case .stopped:
            return "icon_paused"
        case .revoked,
                .failed:
            return "icon_stop"
        }
    }
    
    var textColor: Color {
        switch self {
        case .created:
            return .buttonDefault
        case .signed:
            return .buttonDefault
        case .invalid:
            return .textLight
        case .active:
            return .buttonConfirm
        case .stopped:
            return .buttonDanger
        case .revoked,
                .failed,
                .expired,
                .unknown:
            return .textError
        }
    }
}
