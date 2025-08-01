//
//  ValidIdentificationNumberLengthRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidLengthRule: ValidationRule {
    // MARK: - Properties
    private var length: Int = 0
    
    // MARK: - Init
    init(length: Int) {
        self.length = length
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        if string.count == length {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .invalidLength(length: length))
        }
    }
}
