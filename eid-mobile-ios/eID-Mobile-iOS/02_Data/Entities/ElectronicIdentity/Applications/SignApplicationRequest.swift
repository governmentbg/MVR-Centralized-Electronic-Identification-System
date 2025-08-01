//
//  SignApplicationRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 13.05.24.
//

import Foundation


struct SignApplicationRequest: Codable {
    // MARK: - Properties
    var otpCode: String
    var mobileApplicationInstanceId: String
    var firebaseId: String
    var forceUpdate: Bool = true
}
