//
//  ApplicationsRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 1.03.24.
//

import Foundation


struct ApplicationsRequest: PaginationMPOZEIRequest, Codable {
    // MARK: - Properties
    var page: Int
    var size: Int = 20
    /// FIlters
    var status: ApplicationStatus? = nil
    var id: String? = nil
    var applicationNumber: String? = nil
    var deviceId: String? = nil
    var createdDateFrom: String? = nil
    var createdDateTo: String? = nil
    var applicationType: EIDApplicationType? = nil
    var eidAdministratorId: String? = nil
    /// Sort
    var sort: String? = nil
}
