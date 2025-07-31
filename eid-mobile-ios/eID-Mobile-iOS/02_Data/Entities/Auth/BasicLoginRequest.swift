//
//  BasicLoginRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.07.24.
//

import Foundation


struct BasicLoginRequest: Codable {
    // MARK: - Properties
    var email: String
    var password: String
    var cliendId: String = "eid_ios_app"
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case email
        case password
        case cliendId = "client_id"
    }
}
