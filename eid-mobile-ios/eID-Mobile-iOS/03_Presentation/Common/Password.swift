//
//  Password.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 6.06.24.
//

import SwiftUI


struct Password: Validation {
    // MARK: - Properties
    var value: String
    var comparePassword: String?
    var isConfirm: Bool
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        if isConfirm {
            if let comparePassword = comparePassword {
                return value.isValid(rules: [ValidRepeatPasswordRule(password: comparePassword)])
            } else {
                return ValidationResult(isValid: false, error: .passwordMismatch)
            }
        } else {
            return value.isValid(rules: [ValidPasswordRule()])
        }
    }
}
