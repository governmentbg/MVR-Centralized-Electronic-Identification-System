//
//  CreateApplicationResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 13.03.24.
//

import Foundation


struct CreateApplicationResponse: Codable {
    // MARK: - Properties
    var id: String
    var status: ApplicationStatus?
    var eidAdministratorName: String?
    var payment: [Payment]?
    /// Private properties to build a custom payment model
    private var paymentAccessCode: String?
    private var fee: Double?
    private var feeCurrency: String?
    private var secondaryFee: Double?
    private var secondaryFeeCurrency: String?
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case id
        case status
        case eidAdministratorName
        case fee
        case feeCurrency
        case secondaryFee
        case secondaryFeeCurrency
        case paymentAccessCode
    }
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        id =  try container.decode(String.self, forKey: .id)
        status =  try container.decodeIfPresent(ApplicationStatus.self, forKey: .status)
        eidAdministratorName =  try container.decodeIfPresent(String.self, forKey: .eidAdministratorName)
        
        if let fee = try container.decodeIfPresent(Double.self, forKey: .fee),
           let feeCurrency = try container.decodeIfPresent(String.self, forKey: .feeCurrency),
           let secondaryFee = try container.decodeIfPresent(Double.self, forKey: .secondaryFee),
           let secondaryFeeCurrency = try container.decodeIfPresent(String.self, forKey: .secondaryFeeCurrency) {
            let paymentAccessCode = try container.decodeIfPresent(String.self, forKey: .paymentAccessCode) ?? ""
            
            let feePayment = Payment(fee: fee, feeCurrency: feeCurrency, accessCode: paymentAccessCode)
            let secondaryFeePayment = Payment(fee: secondaryFee, feeCurrency: secondaryFeeCurrency, accessCode: paymentAccessCode)
            payment = [feePayment, secondaryFeePayment]
        }
    }
}

struct Payment: Equatable, Hashable {
    var fee: Double?
    var feeCurrency: String?
    var accessCode: String?
}
