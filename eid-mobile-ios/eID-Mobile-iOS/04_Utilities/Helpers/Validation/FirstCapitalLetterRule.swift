//
//  FirstCapitalLetterRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 5.06.24.
//

import Foundation


class FirstCapitalLetterRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let firstCharacter = string.first
        return firstCharacter?.isUppercase == true ? .valid : ValidationResult(isValid: false, error: .firstCapitalLetter)
    }
}
