//
//  ChangeCertificateStatusRequest.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 25.04.24.
//

import Foundation


struct ChangeCertificateStatusRequest: Codable {
    // MARK: - Properties
    var firstName: String
    var secondName: String
    var lastName: String
    var firstNameLatin: String
    var secondNameLatin: String
    var lastNameLatin: String
    var dateOfBirth: String
    var eidAdministratorId: String
    var eidAdministratorOfficeId: String
    var citizenIdentifierNumber: String
    var citizenIdentifierType: IdentifierType
    var applicationType: EIDApplicationType
    var deviceType: String
    var citizenship: String
    var personalIdentityDocument: PersonalIdentityDocument
    var reasonId: String?
    var reasonText: String?
    var certificateId: String
}
