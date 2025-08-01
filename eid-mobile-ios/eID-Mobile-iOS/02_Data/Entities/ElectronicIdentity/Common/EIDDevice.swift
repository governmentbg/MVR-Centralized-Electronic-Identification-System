//
//  EIDDevice.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.07.24.
//

import Foundation


typealias EIDDevicesResponse = [EIDDevice]


struct EIDDevice: Codable, Hashable, Validation {
    // MARK: - Properties
    var id: String
    var name: String
    var type: String
    var description: String
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        return id.isValid(rules: [NotEmptyRule()])
    }
}

extension EIDDevice {
    static let `default`: EIDDevice = EIDDevice(id: "", name: "", type: "", description: "")
}

extension EIDDevice {
    static let chipCardID = "bc9f97f8-b004-4b61-ac85-7d1a7cb05f14"
    static let mobileDeviceID = "cf2a0594-108d-487f-b588-67033e1a0555"
}
