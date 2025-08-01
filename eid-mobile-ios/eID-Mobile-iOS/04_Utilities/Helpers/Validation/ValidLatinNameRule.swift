//
//  ValidLatinNameRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 5.06.24.
//

import Foundation


class ValidLatinNameRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let charset = CharacterSet(charactersIn: "- ’'`")
            .union(.uppercaseLatinLetters)
            .union(.lowercaseLatinLetters)
        return charset.isSuperset(of: CharacterSet(charactersIn: string)) ? .valid : ValidationResult(isValid: false, error: .invalidLatinName)
    }
}
