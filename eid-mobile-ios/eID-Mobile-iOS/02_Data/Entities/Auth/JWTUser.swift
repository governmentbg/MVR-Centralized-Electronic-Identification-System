//
//  JWTUser.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 25.10.23.
//

import Foundation


struct JWTUser: Codable {
    // MARK: - Properties
    var citizenProfileId: String
    var eidenityId: String?
    var citizenIdentifierType: String?
    var citizenIdentifier: String?
    var name: String?
    var exp: Int?
    var iat: Int?
    var jti: String?
    var iss: String?
    var sub: String?
    var typ: String?
    var azp: String?
    var sessionState: String?
    var acr: UserACR?
    var allowedOrigins: [String]?
    var scope: String?
    var sid: String?
    var emailVerified: Bool
    var preferredUsername: String?
    var locale: String?
    /// Latin name
    var givenNameLatin: String?
    var middleNameLatin: String?
    var familyNameLatin: String?
    /// Cyrillic name
    var givenNameCyrillic: String?
    var middleNameCyrillic: String?
    var familyNameCyrillic: String?
    var email: String?
    
    /// Computed
    var nameCyrillic: String {
        var name = givenNameCyrillic ?? ""
        if let middleName = middleNameCyrillic {
            name += " \(middleName)"
        }
        if let familyName = familyNameCyrillic {
            name += " \(familyName)"
        }
        return name
    }
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case citizenProfileId = "citizen_profile_id"
        case eidenityId = "eidenity_id"
        case citizenIdentifierType = "citizen_identifier_type"
        case citizenIdentifier = "citizen_identifier"
        case name
        case exp
        case iat
        case jti
        case iss
        case sub
        case typ
        case azp
        case sessionState = "session_state"
        case acr
        case allowedOrigins = "allowed-origins"
        case scope
        case sid
        case emailVerified = "email_verified"
        case preferredUsername = "preferred_username"
        case locale = "preferred_language"
        case givenNameLatin = "given_name"
        case middleNameLatin = "middle_name"
        case familyNameLatin = "family_name"
        case givenNameCyrillic = "given_name_cyrillic"
        case middleNameCyrillic = "middle_name_cyrillic"
        case familyNameCyrillic = "family_name_cyrillic"
        case email
    }
}

enum UserACR: String, Codable {
    case low = "eid_low"
    case substantial = "eid_substantial"
    case high = "eid_high"
}
