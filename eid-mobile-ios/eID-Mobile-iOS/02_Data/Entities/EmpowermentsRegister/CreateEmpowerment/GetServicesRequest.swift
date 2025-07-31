//
//  GetServicesRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation


struct GetServicesRequest: Codable, PaginationRequest {
    // MARK: - Properties
    var providerid: String
    var includeEmpowermentOnly: Bool = true
    var pageSize: Int = 1000
    var pageIndex: Int
}
