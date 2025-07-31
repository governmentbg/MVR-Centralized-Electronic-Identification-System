//
//  GetNotificationTypesRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import Foundation


struct GetNotificationTypesRequest: Codable, PaginationRequest {
    // MARK: - Properties
    var systemName: String? = nil
    var pageSize: Int = 50
    var pageIndex: Int
}
