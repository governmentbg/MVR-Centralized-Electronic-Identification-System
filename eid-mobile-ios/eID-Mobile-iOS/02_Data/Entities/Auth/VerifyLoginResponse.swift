//
//  VerifyLoginResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.07.24.
//

import Foundation


struct VerifyLoginResponse: Codable {
    // MARK: - Properties
    var message: String
    var statusCode: Int
}
