//
//  GetNotificationTypesResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import Foundation


struct GetNotificationTypesPageResponse: Codable, PaginationResponse {
    // MARK: - Properties
    let pageIndex: Int
    let totalItems: Int
    var data: [NotificationTypeResponse]
}

struct NotificationTypeResponse: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var name: String
    var modifiedOn: String?
    var modifiedBy: String?
    var isApproved: Bool
    var isDeleted: Bool
    var translations: [NotificationTypeTranslationResponse]?
    var events: [NotificationEventResponse]
    
    // MARK: - Hashable
    static func == (lhs: NotificationTypeResponse, rhs: NotificationTypeResponse) -> Bool {
        return lhs.id == rhs.id
    }
}

struct NotificationTypeTranslationResponse: Codable, Hashable {
    // MARK: - Properties
    var language: Language
    var name: String
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        name =  try container.decode(String.self, forKey: .name)
        
        let stringLanguage = try container.decodeIfPresent(String.self, forKey: .language) ?? ""
        language = Language(rawValue: stringLanguage) ?? .bg
    }
}

struct NotificationEventResponse: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var code: String
    var modifiedOn: String?
    var modifiedBy: String?
    var isMandatory: Bool
    var isDeleted: Bool
    var translations: [NotificaionEventTranslationResponse]?
    
    // MARK: - Hashable
    static func == (lhs: NotificationEventResponse, rhs: NotificationEventResponse) -> Bool {
        return lhs.id == rhs.id
    }
}

struct NotificaionEventTranslationResponse: Codable, Hashable {
    // MARK: - Properties
    var language: Language
    var shortDescription: String
    var description: String
}

extension NotificationTypeResponse {
    func getLocalizedName(language: Language = .bg) -> String {
        return self.translations?.filter { $0.language == language }.first?.name ?? ""
    }
}

extension NotificationEventResponse {
    func getLocalizedShortDescription(language: Language = .bg) -> String {
        return self.translations?.filter { $0.language == language }.first?.shortDescription ?? ""
    }
}
