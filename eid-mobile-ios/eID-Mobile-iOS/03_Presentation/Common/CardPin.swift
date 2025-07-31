//
//  CardPin.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 28.06.24.
//

import SwiftUI


struct CardPin: Validation {
    // MARK: - Properties
    var value: String
    var comparePin: String?
    var isConfirm: Bool
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        if isConfirm {
            if let comparePin = comparePin {
                return value.isValid(rules: [NotEmptyRule(),
                                             DigitsOnlyRule(),
                                             ValidLengthRule(length: Constants.PIN.length),
                                             ValidRepeatPinRule(pin: comparePin)])
            } else {
                return ValidationResult(isValid: false, error: .passwordMismatch)
            }
        } else {
            return value.isValid(rules: [DigitsOnlyRule(),
                                         ValidLengthRule(length: Constants.PIN.length)])
        }
    }
}
