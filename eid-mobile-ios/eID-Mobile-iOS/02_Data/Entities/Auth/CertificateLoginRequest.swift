//
//  CertificateLoginRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.07.24.
//

import Foundation


struct CertificateLoginRequest: Codable {
    // MARK: - Properties
    var signedChallenge: SignedChallenge?
    var cliendId: String = "eid_ios_app"
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case signedChallenge
        case cliendId = "client_id"
    }
}

struct SignedChallenge: Codable {
    var signature: String
    var challenge: String
    var certificate: String
    var certificateChain: [String]
}
