//
//  LogEventRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 18.10.24.
//

import Foundation


struct LogEventRequest: Codable {
    // MARK: - Properties
    var eventType: LogEventType
    var eventPayload: LogEventPayload
}


typealias LogEventPayload = [String: String]
