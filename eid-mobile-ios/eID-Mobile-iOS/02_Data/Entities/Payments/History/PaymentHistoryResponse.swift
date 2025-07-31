//
//  PaymentHistoryResponse.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import Foundation

struct PaymentHistoryResponse: Codable, Hashable {
    // MARK: - Properties
    var ePaymentId: String?
    var citizenProfileId: String?
    var createdOn: String?
    var paymentDeadline: String?
    var paymentDate: String?
    var status: PaymentHistoryStatus?
    var accessCode: String?
    var registrationTime: String?
    var referenceNumber: String?
    var reason: PaymentHistoryReason?
    var lastSync: String?
    var payment: [Payment]?
    private var currency: String?
    private var amount: Double?
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case ePaymentId
        case citizenProfileId
        case createdOn
        case paymentDeadline
        case paymentDate
        case status
        case accessCode
        case registrationTime
        case referenceNumber
        case reason
        case currency
        case amount
        case lastSync
    }
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        ePaymentId =  try container.decodeIfPresent(String.self, forKey: .ePaymentId)
        citizenProfileId =  try container.decodeIfPresent(String.self, forKey: .citizenProfileId)
        createdOn =  try container.decodeIfPresent(String.self, forKey: .createdOn)
        paymentDeadline =  try container.decodeIfPresent(String.self, forKey: .paymentDeadline)
        paymentDate =  try container.decodeIfPresent(String.self, forKey: .paymentDate)
        status =  try container.decode(PaymentHistoryStatus.self, forKey: .status)
        accessCode =  try container.decode(String.self, forKey: .accessCode)
        registrationTime =  try container.decodeIfPresent(String.self, forKey: .registrationTime)
        referenceNumber =  try container.decodeIfPresent(String.self, forKey: .referenceNumber)
        reason =  try container.decodeIfPresent(PaymentHistoryReason.self, forKey: .reason)
        lastSync =  try container.decodeIfPresent(String.self, forKey: .lastSync)
        
        if let fee = try container.decodeIfPresent(Double.self, forKey: .amount),
           let feeCurrency = try container.decodeIfPresent(String.self, forKey: .currency) {
            payment = [Payment(fee: fee, feeCurrency: feeCurrency, accessCode: "")]
        }
    }
}
