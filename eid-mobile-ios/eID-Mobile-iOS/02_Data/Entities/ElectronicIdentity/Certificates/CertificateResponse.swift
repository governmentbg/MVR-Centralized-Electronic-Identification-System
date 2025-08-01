//
//  CertificateResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.02.24.
//

import Foundation


struct CertificatesPageResponse: PaginationMPOZEIResponse, Codable {
    // MARK: - Properties
    var number: Int
    var totalPages: Int
    var totalElements: Int
    var content: [CertificateResponse]
}

struct CertificateResponse: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var status: CertificateStatus?
    var eidentityId: String?
    var validityFrom: String
    var validityUntil: String
    var serialNumber: String
    var deviceId: String
    var eidAdministratorId: String
    var levelOfAssurance: String
    var isExpiring: Bool?
    var alias: String?
}
