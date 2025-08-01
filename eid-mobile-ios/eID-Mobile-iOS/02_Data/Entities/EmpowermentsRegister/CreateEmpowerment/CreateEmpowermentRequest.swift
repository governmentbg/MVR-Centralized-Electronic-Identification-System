//
//  CreateEmpowermentRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 6.11.23.
//

import Foundation


struct CreateEmpowermentRequest: Codable {
    // MARK: - Properties
    var onBehalfOf: String
    var issuerPosition: String?
    var uid: String?
    var uidType: IdentifierType
    var authorizerUids: [EmpowermentAuthorizer]
    var name: String?
    var empoweredUids: [UserIdentifier]
    var typeOfEmpowerment: Int
    var providerId: String
    var providerName: String
    var serviceId: Int
    var serviceName: String
    var volumeOfRepresentation: [ServiceScope]
    var startDate: String
    var expiryDate: String?
}
