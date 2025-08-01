//
//  GenerateApplicationXmlRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 6.03.24.
//

import Foundation


struct GenerateApplicationXmlRequest: Codable {
    // MARK: - Properties
    var firstName: String
    var secondName: String
    var lastName: String
    var firstNameLatin: String
    var secondNameLatin: String
    var lastNameLatin: String
    var applicationType: EIDApplicationType
    var deviceId: String
    var citizenship: String
    var citizenIdentifierNumber: String
    var citizenIdentifierType: IdentifierType
    var personalIdentityDocument: PersonalIdentityDocument
    var eidAdministratorId: String
    var eidAdministratorOfficeId: String
    var dateOfBirth: String
    var reasonId: String?
    var reasonText: String?
    var certificateId: String?
}

struct PersonalIdentityDocument: Codable {
    // MARK: - Properties
    var identityNumber: String
    var identityType: String
    var identityIssueDate: String
    var identityValidityToDate: String
}
