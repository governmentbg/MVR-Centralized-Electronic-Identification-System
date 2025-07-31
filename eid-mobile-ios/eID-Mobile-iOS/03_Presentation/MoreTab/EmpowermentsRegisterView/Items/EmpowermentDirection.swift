//
//  EmpowermentDirection.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 27.11.23.
//

import Foundation

enum EmpowermentDirection {
    case fromMe
    case toMe
    case fromMeEIK
    
    var title: String {
        switch self {
        case .fromMe:
            return "empowerments_from_me_title"
        case .toMe:
            return "empowerments_to_me_title"
        case .fromMeEIK:
            return ""
        }
    }
    
    var subtitle: String {
        switch self {
        case .fromMe:
            return "empowerments_from_me_subtitle"
        case .toMe:
            return "empowerments_to_me_subtitle"
        case .fromMeEIK:
            return ""
        }
    }
    
    var iconName: String {
        switch self {
        case .fromMe:
            return "icon_empowerments_from_me"
        case .toMe:
            return "icon_empowerments_to_me"
        case .fromMeEIK:
            return ""
        }
    }
    
    var filterStatuses: [EmpowermentStatus] {
        switch self {
        case .fromMe,
                .fromMeEIK:
            return [.active,
                    .collectingAuthorizerSignatures,
                    .expired,
                    .disagreementDeclared,
                    .withdrawn,
                    .denied,
                    .unconfirmed]
        case .toMe:
            return [.active,
                    .expired,
                    .disagreementDeclared,
                    .withdrawn,
                    .unconfirmed]
        }
    }
}
