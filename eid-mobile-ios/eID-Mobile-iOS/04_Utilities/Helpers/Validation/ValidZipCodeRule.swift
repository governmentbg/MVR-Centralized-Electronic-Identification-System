//
//  ValidZipCodeRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidZipCodeRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let postalcodeRegex = "^[0-9]{4}(-[0-9]{4})?$"
        let pinPredicate = string.applyRegEx(postalcodeRegex)
        if pinPredicate.evaluate(with: string) {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .invalidZipCode)
        }
    }
}
