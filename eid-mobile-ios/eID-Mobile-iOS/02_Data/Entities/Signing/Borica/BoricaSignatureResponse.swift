//
//  BoricaSignatureResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


struct BoricaSignatureResponse: Codable {
    // MARK: - Properties
    var responseCode: String
    var code: String
    var message: String
    var fileName: String
    var contentType: String
    var content: String?
}
