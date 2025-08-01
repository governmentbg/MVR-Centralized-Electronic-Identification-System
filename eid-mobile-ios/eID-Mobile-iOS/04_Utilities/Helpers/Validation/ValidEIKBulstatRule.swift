//
//  ValidEIKBulstatRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidEIKBulstatRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let isDigitsOnlyResult = string.isValid(rules: [DigitsOnlyRule()])
        guard isDigitsOnlyResult.isValid else { return isDigitsOnlyResult }
        
        if isValidLength(string) {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .invalidEIKLength)
        }
    }
    
    // MARK: - Private Helpers
    private func isValidLength(_ eikOrBulstat: String) -> Bool {
        return eikOrBulstat.count == 9 || eikOrBulstat.count == 13
    }
}
