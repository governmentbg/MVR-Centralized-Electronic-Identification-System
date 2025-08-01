//
//  ValidPasswordRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidPasswordRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let passwordRegEx = #"(?=.*[A-Za-z].*[A-Za-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()\-_=+\\|\[\]{};:'",.<>?]).{8,}"#
        let passwordPred = string.applyRegEx(passwordRegEx)
        if passwordPred.evaluate(with: string) {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .invalidPassword)
        }
    }
}
