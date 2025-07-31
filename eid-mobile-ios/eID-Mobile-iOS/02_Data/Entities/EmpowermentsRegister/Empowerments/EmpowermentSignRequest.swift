//
//  EmpowermentSignRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 19.12.23.
//

import Foundation


struct EmpowermentSignRequest: Codable {
    // MARK: - Properties
    var empowermentId: String
    var signatureProvider: SignatureProvider
    var detachedSignature: String
}
