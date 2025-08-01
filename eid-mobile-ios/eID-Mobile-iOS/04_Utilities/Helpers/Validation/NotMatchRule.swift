//
//  MatchRule.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 19.03.25.
//

import Foundation

class NotMatchRule: ValidationRule {
    
    // MARK: - Properties
    private var original: String = ""
    
    // MARK: - Init
    init(original: String) {
        self.original = original
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        return if string != original {
            .valid
        } else {
            ValidationResult(isValid: false, error: .noError)
        }
    }
}
