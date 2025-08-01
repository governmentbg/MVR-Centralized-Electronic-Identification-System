//
//  BoricaCheckUserResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.01.24.
//

import Foundation


struct BoricaCheckUserResponse: Codable {
    // MARK: - Properties
    var responseCode: String
    var code: String
    var message: String
    var data: BoricaCertificateData
}

struct BoricaCertificateData: Codable {
    // MARK: - Properties
    var certReqId: String
    var devices: [String]
    var encodedCert: String
}
