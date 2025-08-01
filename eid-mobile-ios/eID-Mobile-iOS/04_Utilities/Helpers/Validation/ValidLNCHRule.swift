//
//  ValidLNCHRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidLNCHRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let isDigitsOnlyResult = string.isValid(rules: [DigitsOnlyRule()])
        guard isDigitsOnlyResult.isValid else { return ValidationResult(isValid: false, error: .invalidLnch) }
        
        let isValidLength = string.isValid(rules: [ValidLengthRule(length: 10)])
        guard isValidLength.isValid else { return ValidationResult(isValid: false, error: .invalidLnchLength) }
        
        let lastDigit = calculateLastDigit(string)
        return string.isValid(rules: [ValidLastDigitRule(lastDigit: lastDigit, type: .lnch)])
    }
    
    // MARK: - Private helpers
    private func calculateLastDigit(_ egn: String) -> Int {
        var lastDigit = 0
        let weights = [2, 4, 8, 5, 10, 9, 7, 3, 6]
        for (index, _) in weights.enumerated() {
            lastDigit += weights[index] * (Int(egn.substring(from: index, take: 1)) ?? 0)
        }
        lastDigit %= 11
        lastDigit %= 10
        return lastDigit
    }
}
