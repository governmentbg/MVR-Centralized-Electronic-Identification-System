//
//  ValidCompoundNameRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 28.05.25.
//

import Foundation


class ValidCompoundNameRule: ValidationRule {
    // MARK: - Properties
    private var otherName: String
    
    // MARK: - Init
    init(otherName: String) {
        self.otherName = otherName
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        if string.isEmpty && otherName.isEmpty {
            return ValidationResult(isValid: false, error: .invalidCompoundName)
        } else {
            return ValidationResult.valid
        }
    }
}
