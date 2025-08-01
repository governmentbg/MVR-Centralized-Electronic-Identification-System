//
//  LogsFromMeResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


struct LogsFromMePageResponse: Codable, CursorPaginationResponse {
    // MARK: - Properties
    var searchAfter: [Int]
    var data: [LogFromMe]
}

struct LogFromMe: Logs, Codable, Hashable {
    // MARK: - Properties
    var id: UUID
    var eventId: String
    var eventDate: String
    var eventType: String
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case eventId
        case eventDate
        case eventType
    }
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        id = UUID()
        eventId =  try container.decode(String.self, forKey: .eventId)
        eventDate =  try container.decode(String.self, forKey: .eventDate)
        eventType =  try container.decode(String.self, forKey: .eventType)
    }
}
