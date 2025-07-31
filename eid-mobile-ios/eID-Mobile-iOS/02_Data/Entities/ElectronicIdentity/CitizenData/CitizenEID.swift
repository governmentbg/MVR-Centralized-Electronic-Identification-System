//
//  CitizenEID.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.04.24.
//

import Foundation


struct CitizenEID: Codable {
    // MARK: - Properties
    let eidentityId: String?
    let citizenProfileId: String?
    let active: Bool?
    var firstName: String?
    var secondName: String?
    var lastName: String?
    var firstNameLatin: String?
    var secondNameLatin: String?
    var lastNameLatin: String?
    var citizenIdentifierNumber: String?
    var citizenIdentifierType: IdentifierType?
    var email: String?
    var phoneNumber: String?
    var is2FaEnabled: Bool?
}
