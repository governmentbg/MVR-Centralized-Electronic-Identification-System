//
//  ValidEGNRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidEGNRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let isDigitsOnlyResult = string.isValid(rules: [DigitsOnlyRule()])
        guard isDigitsOnlyResult.isValid else { return ValidationResult(isValid: false, error: .invalidEgn) }
        
        let isValidLength = string.isValid(rules: [ValidLengthRule(length: 10)])
        guard isValidLength.isValid else { return ValidationResult(isValid: false, error: .invalidEgnLength) }
        
        let lastDigit = calculateLastDigit(string)
        let isValidLastDigitResult = string.isValid(rules: [ValidLastDigitRule(lastDigit: lastDigit, type: .egn)])
        guard isValidLastDigitResult.isValid else { return isValidLastDigitResult }
        
        if isAdult(string) {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .underaged)
        }
    }
    
    // MARK: - Private Helpers
    private func getDateByEGN(egn: String) -> Date? {
        if egn.count != 10 {
            return nil
        }
        let controlValueYear = Int(egn[..<egn.index(egn.startIndex, offsetBy: 2)]) ?? 0
        let controlValueMonth = Int(egn[egn.index(egn.startIndex, offsetBy: 2)..<egn.index(egn.startIndex, offsetBy: 4)]) ?? 0
        let controlValueDate = String(egn[egn.index(egn.startIndex, offsetBy: 4)..<egn.index(egn.startIndex, offsetBy: 6)])
        var month = controlValueMonth
        var baseYear = 1900
        if controlValueMonth > 40 {
            month -= 40
            baseYear = 2000
        } else if controlValueMonth > 20 {
            baseYear = 1800
            month -= 20
        }
        let year = baseYear + controlValueYear
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "yyyy/MM/dd"
        if let egnToDate = dateFormatter.date(from: "\(year)/\(month)/\(controlValueDate)") {
            return egnToDate
        } else {
            return nil
        }
    }
    
    private func isAdult(_ egn: String) -> Bool {
        let eighteenYearsAgo = Date.now.addYear(n: -18).startOfDay
        guard let egnToDate = getDateByEGN(egn: egn) else {
            return false
        }
        return egnToDate <= eighteenYearsAgo
    }
    
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
