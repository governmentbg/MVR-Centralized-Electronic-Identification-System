//
//  ChangeCitizenPhoneRequest.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.06.24.
//

import Foundation


struct ChangeCitizenInformationRequest: Codable {
    // MARK: - Properties
    var firstName: String
    var secondName: String
    var lastName: String
    var firstNameLatin: String
    var secondNameLatin: String
    var lastNameLatin: String
    var phoneNumber: String
    var is2FaEnabled: Bool
}
