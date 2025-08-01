//
//  ValidMinimumLengthRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 14.05.24.
//

import Foundation


class ValidMinimumLengthRule: ValidationRule {
    // MARK: - Properties
    private var length: Int = 0
    
    // MARK: - Init
    init(min: Int) {
        length = min
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        return string.count >= length ? ValidationResult.valid : ValidationResult(isValid: false, error: .invalidMinimumLength(length: length))
    }
}
