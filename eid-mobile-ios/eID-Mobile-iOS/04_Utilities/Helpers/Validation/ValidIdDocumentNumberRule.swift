//
//  ValidIdDocumentNumberRule.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 29.08.24.
//

import Foundation


class ValidIdDocumentNumberRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let charset = CharacterSet.digits.union(.uppercaseLatinLetters)
        return charset.isSuperset(of: CharacterSet(charactersIn: string)) ? .valid : ValidationResult(isValid: false, error: .invalidIdDocumentNumber)
    }
}


class ValidNewIdDocumentNumberRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let enteredStringCharset = CharacterSet(charactersIn: string)
        let hasDigits = !enteredStringCharset.intersection(.digits).isEmpty
        let hasLetters = !enteredStringCharset.intersection(.uppercaseLatinLetters).isEmpty
        let allAllowed = CharacterSet.digits.union(.uppercaseLatinLetters)
        return allAllowed.isSuperset(of: enteredStringCharset) && hasDigits && hasLetters ? .valid : ValidationResult(isValid: false, error: hasLetters ? .invalidIdDocumentNumber : .invalidNewIdDocumentNumber)
    }
}
