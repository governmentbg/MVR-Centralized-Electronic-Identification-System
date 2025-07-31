//
//  EurotrustStatusResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 18.01.24.
//

import Foundation


struct EurotrustStatusResponse: Codable {
    // MARK: - Properties
    var status: EurotrustSignatureStatus
    var isProcessing: Bool
}

enum EurotrustSignatureStatus: Int, Codable {
    case none = 0
    case pending = 1
    case signed = 2
    case rejected = 3
    case expired = 4
    case failed = 5
    case withdrawn = 6
    case undeliverable = 7
    case onHold = 99
}
