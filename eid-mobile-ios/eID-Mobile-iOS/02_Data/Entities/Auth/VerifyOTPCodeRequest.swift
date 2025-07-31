//
//  VerifyOTPCodeRequest.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.04.25.
//


import Foundation

struct VerifyOTPCodeRequest: Codable {
    // MARK: - Properties
    let sessionId: String
    let otp: String
}
