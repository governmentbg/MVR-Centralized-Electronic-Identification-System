//
//  RegisterCitizenRequest.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 14.05.24.
//

import Foundation


struct RegisterCitizenRequest: Codable {
    // MARK: - Properties
    var firstName: String
    var secondName: String?
    var lastName: String?
    var firstNameLatin: String
    var secondNameLatin: String?
    var lastNameLatin: String?
    var email: String
    var phoneNumber: String?
    var baseProfilePassword: String
    var matchingPassword: String
}
