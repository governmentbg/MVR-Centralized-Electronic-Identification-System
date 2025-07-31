//
//  Name.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 6.06.24.
//

import SwiftUI


struct Name: Validation {
    // MARK: - Properties
    var value: String
    // MARK: - Flags
    var isLatin: Bool
    var firstCapitalValidation: Bool = false
    var isMandatory: Bool = false
    // MARK: - Compound name propertirs
    var isCompoundName: Bool = false
    var otherCompoundName: String?
    // MARK: - Additional Rules
    var additionalRules: [ValidationRule] = []
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        if isLatin {
            return value.isValid(rules: validNameRules + [ValidLatinNameRule()] + additionalRules)
        } else {
            return value.isValid(rules: validNameRules + [ValidNameRule()] + additionalRules)
        }
    }
    // MARK: - Private Properties
    private var validNameRules: [ValidationRule] {
        let firstNameRule = firstCapitalValidation ? [FirstCapitalLetterRule()] : []
        let isMandatoryRule = isMandatory ? [NotEmptyRule()] : []
        let isValidCompoundNameRule = isCompoundName ? [ValidCompoundNameRule(otherName: otherCompoundName ?? "")] : []
        let minimumLengthCheck = (1...2).contains(value.count) ? [ValidMinimumLengthRule(min: 3)] : []
        return isMandatoryRule + minimumLengthCheck + firstNameRule + isValidCompoundNameRule
    }
}
