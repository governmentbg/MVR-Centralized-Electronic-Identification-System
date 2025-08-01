//
//  ForgottenPasswordCheckRequest.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.05.24.
//

import Foundation


struct ForgottenPasswordCheckRequest: Codable {
    // MARK: - Properties
    var firstName: String
    var secondName: String?
    var lastName: String
    var email: String
}
