//
//  ValidRepeatPasswordRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 14.05.24.
//

import Foundation


class ValidRepeatPasswordRule: ValidationRule {
    // MARK: - Properties
    private var password: String = ""
    
    // MARK: - Init
    init(password: String) {
        self.password = password
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        return string == password ? ValidationResult.valid : ValidationResult(isValid: false, error: .passwordMismatch)
    }
}
