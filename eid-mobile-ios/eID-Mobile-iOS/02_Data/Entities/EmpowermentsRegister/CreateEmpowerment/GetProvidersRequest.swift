//
//  GetProvidersRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation


struct GetProvidersRequest: Codable, PaginationRequest {
    // MARK: - Properties
    var pageSize: Int = 1000
    var pageIndex: Int
}
