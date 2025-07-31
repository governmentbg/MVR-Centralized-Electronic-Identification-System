//
//  EurotrustSignRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.01.24.
//

import Foundation


struct EurotrustSignRequest: Codable {
    // MARK: - Properties
    var dateExpire: String
    var documents: [EurotrustDocument]
    var uid: String
}

struct EurotrustDocument: Codable {
    // MARK: - Properties
    var content: String
    var fileName: String
    var contentType: String = "text/xml"
}
