//
//  BoricaSignResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


struct BoricaSignResponse: Codable {
    // MARK: - Properties
    var responseCode: String
    var code: String
    var message: String
    var data: BoricaDocumentTransaction
}

struct BoricaDocumentTransaction: Codable {
    // MARK: - Properties
    var callbackId: String
    var validity: Int
}
