//
//  EIDAdministratorOffice.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.07.24.
//

import Foundation


typealias EIDAdministratorOfficesResponse = [EIDAdministratorOffice]


struct EIDAdministratorOffice: Codable, Validation {
    // MARK: - Properties
    var id: String
    var name: String
    var eidAdministratorId: String?
    var location: String?
    var region: String?
    var contact: String?
    var isActive: Bool?
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        return id.isValid(rules: [NotEmptyRule()])
    }
}

extension EIDAdministratorOffice {
    static let `default`: EIDAdministratorOffice =  EIDAdministratorOffice(id: "",
                                                                           name: "",
                                                                           eidAdministratorId: "",
                                                                           location: "",
                                                                           region: "",
                                                                           contact: "",
                                                                           isActive: false)
}
