//
//  GetDeactivatedNotificationTypesResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import Foundation


struct GetDeactivatedNotificationTypesPageResponse: Codable, PaginationResponse {
    // MARK: - Properties
    let pageIndex: Int
    let totalItems: Int
    var data: [DeactivatedNotificationEvent]
}


/** String describing the ID of the deactivated notification type */
typealias DeactivatedNotificationEvent = String
