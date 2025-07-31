//
//  CertificateDetailsResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.02.24.
//

import Foundation


struct CertificateDetailsResponse: Codable {
    // MARK: - Properties
    var id: String?
    var applicationNumber: String?
    var status: CertificateStatus?
    var eidAdministratorId: String?
    var eidAdministratorOfficeId: String?
    var eidAdministratorName: String?
    var eidentityId: String?
    var commonName: String?
    var validityFrom: String?
    var validityUntil: String?
    var createDate: String?
    var serialNumber: String?
    var deviceId: String?
    var levelOfAssurance: CertificateLevelOfAssurance?
    var reasonId: String?
    var reasonText: String?
    var isExpiring: Bool?
    var alias: String?
}
