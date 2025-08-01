//
//  ValidLastDigitRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidLastDigitRule: ValidationRule {
    // MARK: - Properties
    private var lastDigit: Int = 0
    private var type: IdentifierType = .egn
    
    // MARK: - Init
    init(lastDigit: Int, type: IdentifierType) {
        self.lastDigit = lastDigit
        self.type = type
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let result = lastDigit == (Int(string.substring(from: 9, take: 1)) ?? -1)
        if result {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: type == .egn ? .invalidEgn : .invalidLnch)
        }
    }
}
