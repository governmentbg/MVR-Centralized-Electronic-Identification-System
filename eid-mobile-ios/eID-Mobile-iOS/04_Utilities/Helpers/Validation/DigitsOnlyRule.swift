//
//  DigitsOnlyRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class DigitsOnlyRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        if string.rangeOfCharacter(from: CharacterSet.decimalDigits.inverted) == nil {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .invalidDigits)
        }
    }
}
