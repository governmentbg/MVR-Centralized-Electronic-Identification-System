//
//  EnrollCertificateEIDRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.02.24.
//

import Foundation


struct EnrollCertificateEIDRequest: Codable {
    // MARK: - Properties
    var applicationId: String
    var certificateSigningRequest: String
    var certificateAuthorityName: String = "MVR DEV CA"
}
