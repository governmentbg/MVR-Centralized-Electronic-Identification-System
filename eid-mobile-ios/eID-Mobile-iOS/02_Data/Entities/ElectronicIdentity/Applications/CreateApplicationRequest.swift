//
//  CreateApplicationRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 13.03.24.
//

import Foundation


struct CreateApplicationRequest: Codable {
    // MARK: - Properties
    var xml: String
    var signature: String?
    var signatureProvider: String?
}
