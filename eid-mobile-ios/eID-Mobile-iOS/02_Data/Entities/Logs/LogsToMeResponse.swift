//
//  LogsToMeResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


struct LogsToMePageResponse: Codable, CursorPaginationResponse {
    // MARK: - Properties
    var searchAfter: [Int]
    var data: [LogToMe]
    
}

struct LogToMe: Codable, Hashable, Logs {
    // MARK: - Properties
    var id: UUID
    var eventId: String
    var eventDate: String
    var eventType: String
    var requesterSystemId: String?
    var requesterSystemName: String
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case eventId
        case eventDate
        case eventType
        case requesterSystemId
        case requesterSystemName
    }
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        id = UUID()
        eventId =  try container.decode(String.self, forKey: .eventId)
        eventDate =  try container.decode(String.self, forKey: .eventDate)
        eventType =  try container.decode(String.self, forKey: .eventType)
        requesterSystemId =  try container.decodeIfPresent(String.self, forKey: .requesterSystemId)
        requesterSystemName =  try container.decode(String.self, forKey: .requesterSystemName)
    }
}
