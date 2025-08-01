//
//  LeadingTrailingSpaceRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 24.06.24.
//

import Foundation


class LeadingTrailingSpaceRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        return (string.first != " " && string.last != " ") ? .valid : ValidationResult(isValid: false, error: .invalidName)
    }
}
