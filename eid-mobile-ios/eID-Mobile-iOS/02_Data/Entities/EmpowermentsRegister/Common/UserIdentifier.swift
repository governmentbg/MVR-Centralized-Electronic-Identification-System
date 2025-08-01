//
//  UserIdentifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.01.24.
//

import Foundation


struct UserIdentifier: Codable, Hashable {
    // MARK: - Properties
    var uid: String?
    var uidType: IdentifierType?
    var name: String?
    
    var displayValue: String {
        let displayName = name?.isEmpty == true ? "" : " - \(name ?? "")"
        return "\(uidType?.title.localized() ?? ""): \(uid ?? "")\(displayName)"
    }
}
