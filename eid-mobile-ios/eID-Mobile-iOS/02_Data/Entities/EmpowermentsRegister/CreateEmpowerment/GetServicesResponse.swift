//
//  GetServicesResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation


struct GetServicesPageResponse: Codable {
    // MARK: - Properties
    var pageIndex: Int
    var totalItems: Int
    var data: [ServiceResponse]
}

struct ServiceResponse: Codable, Hashable {
    // MARK: - Properties
    var id: String?
    var serviceNumber: Int?
    var name: String?
    var description: String?
    var paymentInfoNormalCost: Int?
    var isEmpowerment: Bool?
    var providerDetailsId: String?
    var providerSectionId: String?
    var isActive: Bool?
    var minimumLevelOfAssurance: ServicesMinimumLevelOfAssurance?
}

enum ServicesMinimumLevelOfAssurance: String, Codable {
    case low = "Low"
    case substantial = "Substantial"
    case high = "High"
}
