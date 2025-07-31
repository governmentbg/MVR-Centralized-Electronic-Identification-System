//
//  IdentifierType.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 14.04.24.
//

import Foundation


enum IdentifierType: String, Codable {
    case lnch = "LNCh"
    case egn = "EGN"
    case fp = "FP"
    case unknown = ""
    
    var title: String {
        switch self {
        case .lnch:
            return "identifier_type_lnch"
        case .egn:
            return "identifier_type_eng"
        case .fp, .unknown:
            return ""
        }
    }
}

extension IdentifierType {
    init(from decoder: any Decoder) throws {
        let container = try decoder.singleValueContainer()
        let rawValue = try container.decode(String.self)
        self = .init(rawValue: rawValue) ?? .unknown
    }
}
