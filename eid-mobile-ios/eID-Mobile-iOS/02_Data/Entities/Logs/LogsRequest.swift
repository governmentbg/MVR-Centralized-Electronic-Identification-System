//
//  LogsRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


struct LogsRequest: Codable, CursorPaginationRequest {
    // MARK: - Properties
    var cursorSize: Int = 20
    var cursorSearchAfter: [String] = []
    var startDate: String?
    var endDate: String?
    var eventTypes: [String]? = nil
}


struct SystemLocalisationsRequest: Codable {
    // MARK: - Properties
    var language: Language
}
