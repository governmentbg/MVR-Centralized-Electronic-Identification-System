//
//  ChangeCitizenPasswordRequest.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.05.24.
//

import Foundation


struct ChangeCitizenPasswordRequest: Codable {
    // MARK: - Properties
    var oldPassword: String
    var newPassword: String
    var confirmPassword: String
}
