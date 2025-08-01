//
//  ServiceDomain.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 1.03.24.
//

import Foundation


enum ServiceDomain: String {
    // MARK: - Domains
    case PG = "PG"
    case MPOZEI = "MPOZEI"
    case ISCEI = "ISCEI"
    case RAEICEI = "RAEICEI"
    case PAYMENT = "PAYMENT"
}

extension ServiceDomain {
    var baseUrl: KeychainKey {
        switch self {
        case .PG:
            return .baseUrlPG
        case .MPOZEI:
            return .baseUrlMPOZEI
        case .ISCEI:
            return .baseUrlISCEI
        case .RAEICEI:
            return .baseUrlRAEICEI
        case .PAYMENT:
            return .baseUrlPAYMENT
        }
    }
}
