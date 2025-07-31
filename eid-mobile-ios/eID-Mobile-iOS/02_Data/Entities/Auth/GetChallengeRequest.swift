//
//  GetChallengeRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.07.24.
//

import Foundation


struct GetChallengeRequest: Codable {
    // MARK: - Properties
    var requestFrom: String? = nil
    var levelOfAssurance: CertificateLevelOfAssurance = .low
}
