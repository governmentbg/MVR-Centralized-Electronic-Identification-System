//
//  EmpowermentAuthorizer.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 13.06.24.
//

import Foundation


struct EmpowermentAuthorizer: Codable, Hashable {
    // MARK: - Properties
    var uid: String
    var uidType: IdentifierType
    var name: String? = nil
    var isIssuer: Bool? = false
    
    var displayValue: String {
        let displayName = name?.isEmpty == true ? "" : " - \(name ?? "")"
        return "\(uidType.title.localized()): \(uid)\(displayName)"
    }
}
