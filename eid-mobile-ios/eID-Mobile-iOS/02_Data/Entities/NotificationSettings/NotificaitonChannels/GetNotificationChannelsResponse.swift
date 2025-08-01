//
//  GetNotificationChannelsResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation


struct GetNotificationChannelsPageResponse: Codable, PaginationResponse {
    // MARK: - Properties
    let pageIndex: Int
    let totalItems: Int
    var data: [NotificationChannelResponse]
}

struct NotificationChannelResponse: Codable, Hashable {
    // MARK: - Properties
    let id: String
    var name: String?
    var price: Double
    var infoUrl: String?
    var translations: [NotificationChannelTranslationResponse]?
}

struct NotificationChannelTranslationResponse: Codable, Hashable {
    // MARK: - Properties
    var language: Language?
    var name: String?
    var description: String?
}

extension NotificationChannelResponse {
    func getLocalizedName(language: Language = .bg) -> String {
        return self.translations?.filter { $0.language == language }.first?.name ?? ""
    }
    
    func getLocalizedDescription(language: Language = .bg) -> String {
        return self.translations?.filter { $0.language == language}.first?.description ?? ""
    }
}



