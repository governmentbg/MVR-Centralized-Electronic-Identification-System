//
//  ValidInput.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 6.06.24.
//

import SwiftUI


struct ValidInput: Validation {
    // MARK: - Properties
    var value: String
    var rules: [ValidationRule]
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        return value.isValid(rules: rules)
    }
}
