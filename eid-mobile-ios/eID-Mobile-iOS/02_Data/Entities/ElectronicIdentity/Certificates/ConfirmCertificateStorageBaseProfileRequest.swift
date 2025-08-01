//
//  ConfirmCertificateStorageBaseProfileRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 14.05.24.
//

import Foundation


struct ConfirmCertificateStorageBaseProfileRequest: Codable {
    // MARK: - Properties
    var otpCode: String
    var status: CertificateStorageStatus = .ok
    var reason: String? = nil
    var reasonText: String? = nil
}
