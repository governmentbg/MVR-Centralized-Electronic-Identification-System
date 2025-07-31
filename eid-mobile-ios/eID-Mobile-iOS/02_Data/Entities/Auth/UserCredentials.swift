//
//  UserCredentials.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.06.25.
//

import Foundation


struct UserCredentials: Codable {
    var email: String
    var password: String
    var pin: String?
    var useBiometrics: Bool = false
}
