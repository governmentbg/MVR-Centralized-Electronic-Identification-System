//
//  GetNotificationChannelsRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation


struct GetNotificationChannelsRequest: Codable, PaginationRequest {
    // MARK: - Properties
    var channelName: String = ""
    var pageSize: Int = 100
    var pageIndex: Int
}
