//
//  ApplicationResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.02.24.
//

import Foundation


struct ApplicationsPageResponse: PaginationMPOZEIResponse, Codable {
    // MARK: - Properties
    var number: Int
    var totalPages: Int
    var totalElements: Int
    var content: [ApplicationResponse]
}

struct ApplicationResponse: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var applicationNumber: String
    var status: ApplicationStatus?
    var createDate: String
    var eidentityId: String
    var eidAdministratorName: String
    var deviceId: String
    var applicationType: EIDApplicationType
    var paymentAccessCode: String?
}
