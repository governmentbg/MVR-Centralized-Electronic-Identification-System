//
//  CertificatesRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.04.24.
//

import Foundation


struct CertificatesRequest: PaginationMPOZEIRequest, Codable {
    // MARK: - Properties
    var page: Int
    var size: Int = 20
    /// FIlters
    var id: String? = nil
    var serialNumber: String? = nil
    var status: CertificateStatus? = nil
    var validityFrom: String? = nil
    var validityUntil: String? = nil
    var deviceId: String? = nil
    var eidAdministratorId: String? = nil
    var alias: String? = nil
    /// Sort
    var sort: String? = nil
}
