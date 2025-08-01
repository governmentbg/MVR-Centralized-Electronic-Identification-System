//
//  AuthResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.07.24.
//

import Foundation


struct LoginResponse: Codable {
    // MARK: - Properties
    var authResponse: AuthResponse?
    var twoFactorResponse: TwoFactorResponse?
    
    // MARK: - Init
    init(from decoder: Decoder) throws {
        let container = try decoder.singleValueContainer()
        
        if let authResponse = try? container.decode(AuthResponse.self) {
            self.authResponse = authResponse
        } else {
            self.twoFactorResponse = try container.decode(TwoFactorResponse.self)
        }
    }
}


struct AuthResponse: Codable {
    // MARK: - Properties
    let accessToken: String
    let expiresIn: Int
    let refreshExpiresIn: Int
    let refreshToken: String
    let tokenType: String
    let notBeforePolicy: Int
    let sessionState: String
    let scope: String
    
    // MARK: - Coding Keys
    enum CodingKeys: String, CodingKey {
        case accessToken
        case expiresIn
        case refreshExpiresIn
        case refreshToken
        case tokenType
        case notBeforePolicy = "not-before-policy"
        case sessionState
        case scope
    }
}


struct TwoFactorResponse: Codable {
    // MARK: - Properties
    let sessionId: String
    let ttl: Int
    var otpCodeLimit: Int?
}
