//
//  EnrollCertificateBaseProfileRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.05.24.
//

import Foundation


struct EnrollCertificateBaseProfileRequest: Codable {
    // MARK: - Properties
    var otpCode: String
    var certificateSigningRequest: String
}
