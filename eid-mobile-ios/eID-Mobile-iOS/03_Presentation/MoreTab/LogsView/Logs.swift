//
//  Untitled.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 18.10.24.
//


// MARK: - Logs Protocol
protocol Logs: Hashable {
    // MARK: - Properties
    var eventId: String { get set }
    var eventDate: String { get set }
    var eventType: String { get set }
}


// MARK: - Logs Descriptions Models
struct LocalisedLog: Codable {
    // MARK: - Properties
    var key: String
    var descriptions: [String: String]
    /// Computed
    var localisedDescription: String {
        let currentLanguage = LanguageManager.preferredLanguage
        return descriptions.first(where: { $0.key == currentLanguage?.rawValue })?.value ?? ""
    }
}
