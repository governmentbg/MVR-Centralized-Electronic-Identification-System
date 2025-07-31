//
//  EurotrustTransactionRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 18.01.24.
//

import Foundation


struct EurotrustTransactionRequest: Codable {
    // MARK: - Properties
    var transactionId: String
    var groupSigning: Bool = false
}
