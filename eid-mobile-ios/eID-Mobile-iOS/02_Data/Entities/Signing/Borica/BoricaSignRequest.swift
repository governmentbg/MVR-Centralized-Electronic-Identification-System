//
//  BoricaSignRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


struct BoricaSignRequest: Codable {
    // MARK: - Properties
    var contents: [BoricaDocument]
    var uid: String
}

struct BoricaDocument: Codable {
    // MARK: - Properties
    var confirmText: String = "Confirm sign"
    var contentFormat: String = "BINARY_BASE64"
    var mediaType: String = "text/xml"
    var data: String
    var fileName: String
    var padesVisualSignature: Bool = true
    var signaturePosition: BoricaSignaturePosition = BoricaSignaturePosition()
}

struct BoricaSignaturePosition: Codable {
    // MARK: - Properties
    var imageHeight: Int = 20
    var imageWidth: Int = 100
    var imageXAxis: Int = 20
    var imageYAxis: Int = 20
    var pageNumber: Int = 1
}
