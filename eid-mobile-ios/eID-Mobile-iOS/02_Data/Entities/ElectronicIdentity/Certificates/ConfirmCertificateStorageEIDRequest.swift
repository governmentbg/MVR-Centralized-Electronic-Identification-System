//
//  ConfirmCertificateStorageEIDRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 14.05.24.
//

import Foundation


struct ConfirmCertificateStorageEIDRequest: Codable {
    // MARK: - Properties
    var applicationId: String
    var status: CertificateStorageStatus = .ok
    var reason: String? = nil
    var reasonText: String? = nil
}
