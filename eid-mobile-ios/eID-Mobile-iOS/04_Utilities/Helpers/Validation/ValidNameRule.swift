//
//  ValidNameRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 13.05.24.
//

import Foundation


class ValidNameRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let charset = CharacterSet(charactersIn: "- ’'`")
            .union(.uppercaseBulgarianCyrillicLetters)
            .union(.lowercaseBulgarianCyrillicLetters)
        return charset.isSuperset(of: CharacterSet(charactersIn: string)) ? .valid : ValidationResult(isValid: false, error: .invalidName)
    }
}
