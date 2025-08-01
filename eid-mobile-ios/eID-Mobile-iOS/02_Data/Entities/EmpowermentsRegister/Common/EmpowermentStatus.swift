//
//  EmpowermentStatus.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.11.23.
//

import SwiftUI


enum EmpowermentStatus: String, Codable, CaseIterable {
    case created = "Created"
    case collectingAuthorizerSignatures = "CollectingAuthorizerSignatures"
    case active = "Active"
    case denied = "Denied"
    case disagreementDeclared = "DisagreementDeclared"
    case collectingWithdrawalSignatures = "CollectingWithdrawalSignatures"
    case withdrawn = "Withdrawn"
    case expired = "Expired"
    case unconfirmed = "Unconfirmed"
    case upcoming = "UpComing"
    case awaitingSignature = "AwaitingSignature"
    case unknown = ""
}

extension EmpowermentStatus {
    init(from decoder: Decoder) throws {
        let container = try decoder.singleValueContainer()
        let rawValue = try container.decode(String.self)
        self = .init(rawValue: rawValue) ?? .unknown
    }
}

extension EmpowermentStatus {
    var title: String {
        switch self {
        case .created:
            return "empowerment_status_created"
        case .collectingAuthorizerSignatures, .awaitingSignature:
            return "empowerment_status_collecting_authorizer_signatures"
        case .active:
            return "empowerment_status_active"
        case .denied:
            return "empowerment_status_denied"
        case .disagreementDeclared:
            return "empowerment_status_disagreement_declared"
        case .collectingWithdrawalSignatures:
            return "empowerment_status_collecting_withdrawal_signatures"
        case .withdrawn:
            return "empowerment_status_withdrawn"
        case .expired:
            return "empowerment_status_expired"
        case .unconfirmed:
            return "empowerment_status_unconfirmed"
        case .upcoming:
            return "empowerment_status_upcoming"
        case .unknown:
            return "status_unknown"
        }
    }
    
    var description: String {
        switch self {
        case .created:
            return "empowerment_status_description_created"
        case .collectingAuthorizerSignatures, .awaitingSignature:
            return "empowerment_status_description_collecting_authorizer_signatures"
        case .active:
            return "empowerment_status_description_active"
        case .denied:
            return "empowerment_status_description_denied"
        case .disagreementDeclared:
            return "empowerment_status_description_disagreement_declared"
        case .collectingWithdrawalSignatures:
            return "empowerment_status_description_collecting_withdrawal_signatures"
        case .withdrawn:
            return "empowerment_status_description_withdrawn"
        case .expired:
            return "empowerment_status_description_expired"
        case .unconfirmed:
            return "empowerment_status_description_unconfirmed"
        case .upcoming:
            return "empowerment_status_description_upcoming"
        case .unknown:
            return "status_unknown"
        }
    }
    
    var iconName: String {
        switch self {
        case .created, .awaitingSignature, .collectingAuthorizerSignatures:
            return "icon_clock"
        case .active:
            return "icon_status_active"
        case .denied:
            return "icon_cross_red"
        case .disagreementDeclared, .unknown:
            return "icon_forbidden"
        case .collectingWithdrawalSignatures:
            return "icon_signature"
        case .withdrawn:
            return "icon_stop"
        case .expired:
            return "icon_expired"
        case .unconfirmed:
            return "icon_info"
        case .upcoming:
            return "icon_calendar_upcoming"
        }
    }
    
    var textColor: Color {
        switch self {
        case .created, .awaitingSignature, .collectingAuthorizerSignatures:
            return .buttonDanger
        case .active:
            return .buttonConfirm
        case .denied:
            return .textError
        case .disagreementDeclared, .unknown:
            return .textError
        case .collectingWithdrawalSignatures:
            return .textError
        case .withdrawn:
            return .textError
        case .expired:
            return .textLight
        case .unconfirmed:
            return .buttonDanger
        case .upcoming:
            return .textActive
        }
    }
    
    var buttonIconName: String {
        switch self {
        case .awaitingSignature, .collectingAuthorizerSignatures:
            return "icon_signature"
        case .created:
            return "icon_clock"
        case .active:
            return "icon_status_active"
        case .denied:
            return "icon_cross_red"
        case .disagreementDeclared, .unknown:
            return "icon_forbidden"
        case .collectingWithdrawalSignatures:
            return "icon_signature"
        case .withdrawn:
            return "icon_stop"
        case .expired:
            return "icon_expired"
        case .unconfirmed:
            return "icon_info"
        case .upcoming:
            return "icon_calendar_upcoming"
        }
    }
}
