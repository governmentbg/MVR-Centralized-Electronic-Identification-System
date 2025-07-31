//
//  LatinCharactersOnlyRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class LatinCharactersOnlyRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let regEx = "([A-Za-z0-9 !\"#$%&'()*+,-./:;<=>?@\\[\\\\\\]^_`{|}~]*)"
        let predicate = string.applyRegEx(regEx)
        if predicate.evaluate(with: string) {
            return ValidationResult.valid
        } else {
            return ValidationResult(isValid: false, error: .invalidLatinName)
        }
    }
}
