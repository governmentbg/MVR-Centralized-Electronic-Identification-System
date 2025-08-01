//
//  ApplicationDetailsResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.03.24.
//

import Foundation


struct ApplicationDetailsResponse: Codable {
    // MARK: - Properties
    var id: String
    var firstName: String
    var secondName: String?
    var lastName: String
    var applicationType: EIDApplicationType
    var certificateId: String?
    var serialNumber: String?
    var status: ApplicationStatus?
    var createDate: String
    var deviceId: String
    var eidAdministratorName: String
    var eidAdministratorOfficeName: String
    var eidentityId: String?
    var email: String?
    var phoneNumber: String?
    var reasonId: String?
    var reasonText: String?
    var xml: String
    var submissionType: ApplicationSubmissionType
    var identityNumber: String?
    var identityType: String?
    var identityIssueDate: String?
    var identityValidityToDate: String?
    var applicationNumber: String?
    var paymentAccessCode: String?
}


enum ApplicationSubmissionType: String, Codable, CaseIterable {
    case desk = "DESK"
    case baseProfile = "BASE_PROFILE"
    case eid = "EID"
    case persoCentre = "PERSO_CENTRE"
}
