//
//  ValidationRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation
import SwiftUI


class ValidationResult: ObservableObject {
    // MARK: - Properties
    @Published var isValid: Bool
    @Published var error: String
    var validationError: ValidationError?
    static let valid = ValidationResult(isValid: true, error: nil)
    var isNotValid: Bool {
        return !isValid
    }
    
    // MARK: - Init
    init(isValid: Bool, error: ValidationError? = nil) {
        self.isValid = isValid
        self.error = error?.description.localized() ?? ""
        self.validationError = error
    }
}

protocol ValidationRule {
    func isValid(for string: String) -> ValidationResult
}

extension String {
    func isValid(rules: [ValidationRule]) -> ValidationResult {
        for rule in rules {
            let validation = rule.isValid(for: self)
            if !validation.isValid {
                return ValidationResult(isValid: false,
                                        error: validation.validationError)
            }
        }
        return ValidationResult.valid
    }
    
    func applyRegEx(_ regEx: String) -> NSPredicate {
        return NSPredicate(format: "SELF MATCHES %@", regEx)
    }
}
