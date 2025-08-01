//
//  ApprovalRequestResponse.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 30.07.24.
//

import Foundation


struct ApprovalRequestResponse: Codable {
    // MARK: - Properties
    var requests: [ApprovalRequest]
    
    init(from decoder: Decoder) throws {
        let values = try decoder.singleValueContainer()
        requests = try values.decode([ApprovalRequest].self)
    }
}

struct ApprovalRequest: Codable, Hashable {
    // MARK: - Properties
    var id: String?
    var username: String?
    var requestFrom: ApprovalRequestFrom?
    var levelOfAssurance: CertificateLevelOfAssurance = .low
    var createDate: String?
}

struct ApprovalRequestFrom: Codable, Hashable {
    // MARK: - Properties
    var type: String?
    var system: ApprovalRequestSystem?
}

struct ApprovalRequestSystem: Codable, Hashable {
    var localisedDescription: String?
    
    init(from decoder: Decoder) throws {
        let container = try decoder.singleValueContainer()
        let dict = try container.decode([String:String].self)
        localisedDescription = dict[LanguageManager.preferredLanguage?.rawValue.uppercased() ?? ""]
    }
}
