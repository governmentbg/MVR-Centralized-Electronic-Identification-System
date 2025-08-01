//
//  CertificateLevelOfAssurance.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.02.24.
//

import Foundation


enum CertificateLevelOfAssurance: String, Codable {
    case low = "LOW"
    case substantial = "SUBSTANTIAL"
    case high = "HIGH"
}

extension CertificateLevelOfAssurance {
    var title: String {
        switch self {
        case .low:
            return "assurance_level_low_title"
        case .substantial:
            return "assurance_level_substantial_title"
        case .high:
            return "assurance_level_high_title"
        }
    }
}
