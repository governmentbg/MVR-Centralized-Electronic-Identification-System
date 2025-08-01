//
//  EnrollCertificateResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.02.24.
//

import Foundation


struct EnrollCertificateResponse: Codable {
    // MARK: - Properties
    let id: String
    var serialNumber: String
    var certificate: String
    var certificateChain: [String]
}
