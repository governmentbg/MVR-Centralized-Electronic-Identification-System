//
//  EurotrustSignResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.01.24.
//

import Foundation


struct EurotrustSignResponse: Codable {
    // MARK: - Properties
    var threadId: String
    var groupSigning: Bool
    var transactions: [EurotrustDocumentTransaction]
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case threadId = "threadID"
        case groupSigning
        case transactions
    }
}

struct EurotrustDocumentTransaction: Codable {
    // MARK: - Properties
    var transactionId: String
    var identificationNumber: String
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case transactionId = "transactionID"
        case identificationNumber
    }
}
