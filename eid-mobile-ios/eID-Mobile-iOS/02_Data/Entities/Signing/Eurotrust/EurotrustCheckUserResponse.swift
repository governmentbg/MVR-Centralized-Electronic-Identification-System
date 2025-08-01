//
//  EurotrustCheckUserResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.01.24.
//

import Foundation


struct EurotrustCheckUserResponse: Codable {
    // MARK: - Properties
    var isRegistered: Bool
    var isIdentified: Bool
    var isRejected: Bool
    var isSupervised: Bool
    var isReadyToSign: Bool
    var hasConfirmedPhone: Bool
    var hasConfirmedEmail: Bool
}
