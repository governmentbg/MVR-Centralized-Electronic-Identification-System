//
//  GetEmpowermentsRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.11.23.
//

import Foundation


struct GetEmpowermentsRequest: Codable, PaginationRequest {
    // MARK: - Properties
    var eik: String?
    /// Page
    var pageSize: Int = 50
    var pageIndex: Int
    /// Sort
    var sortby: EmpowermentSortCriteria? = nil
    var sortDirection: SortDirection? = nil
    /// Filter
    var number: String? = nil
    var status: EmpowermentStatus? = nil
    var onBehalfOf: EmpowermentOnBehalfOf? = nil
    var authorizer: String? = nil
    var providerName: String? = nil
    var serviceName: String? = nil
    var empoweredUids: [UserIdentifier]? = nil
    var validToDate: String? = nil
    var showOnlyNoExpiryDate: Bool? = nil
}
