//
//  EIDAdministrator.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 1.03.24.
//

import Foundation


typealias EIDAdministratorsResponse = [EIDAdministrator]


struct EIDAdministrator: Codable, Validation {
    // MARK: - Properties
    var logo: String?
    var id: String
    var name: String
    var nameLatin: String?
    var address: String?
    var contact: String?
    var homePage: String?
    var eikNumber: String?
    var isActive: Bool?
    var eidManagerFrontOfficeIds: [String?]?
    var deviceIds: [String]?
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        return id.isValid(rules: [NotEmptyRule()])
    }
}

extension EIDAdministrator {
    static let `default`: EIDAdministrator = EIDAdministrator(id: "",
                                                              name: "option_all".localized(),
                                                              nameLatin: "option_all".localized(),
                                                              eikNumber: "",
                                                              isActive: false,
                                                              eidManagerFrontOfficeIds: [],
                                                              deviceIds: [])
}
