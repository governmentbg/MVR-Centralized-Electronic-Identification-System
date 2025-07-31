//
//  IdentityDocumentType.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.06.24.
//

import Foundation


enum IdentityType: String, Codable, CaseIterable {
    case identityCard = "IDENTITY_CARD"
}

extension IdentityType {
    var title: String {
        switch self {
        case .identityCard:
            return "identity_type_title_identity_card"
        }
    }
}

