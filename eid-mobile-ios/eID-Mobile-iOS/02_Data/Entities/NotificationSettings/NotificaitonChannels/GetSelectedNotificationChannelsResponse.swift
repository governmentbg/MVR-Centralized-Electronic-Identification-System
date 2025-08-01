//
//  GetSelectedNotificationChannelsResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.10.23.
//

import Foundation


struct GetSelectedNotificationChannelsPageResponse: Codable, PaginationResponse {
    // MARK: - Properties
    let pageIndex: Int
    let totalItems: Int
    var data: [SelectedNotificationChannelResponse]
}


/** String describing the ID of the selected notification channel */
typealias SelectedNotificationChannelResponse = String
