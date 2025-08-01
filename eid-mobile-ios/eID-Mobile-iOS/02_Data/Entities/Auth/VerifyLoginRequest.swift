//
//  VerifyLoginRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.07.24.
//

import Foundation


struct VerifyLoginRequest: Codable {
    // MARK: - Properties
    var mobileApplicationInstanceId: String
    var firebaseId: String
    var forceUpdate: Bool = true
}
