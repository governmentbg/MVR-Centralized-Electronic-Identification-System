//
//  ValidEmailRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidEmailRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let emailRegEx = #"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"#
        let emailPred = string.applyRegEx(emailRegEx)
        if emailPred.evaluate(with: string) {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .invalidEmail)
        }
    }
}
