//
//  NotEmptyRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 14.05.24.
//

import Foundation


class NotEmptyRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        return string.isEmpty
        ? ValidationResult(isValid: false, error: .emptyString)
        : ValidationResult.valid
    }
}
