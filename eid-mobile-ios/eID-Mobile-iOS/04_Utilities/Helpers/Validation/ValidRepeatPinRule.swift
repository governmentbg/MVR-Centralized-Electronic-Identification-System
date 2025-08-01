//
//  ValidRepeatPinRule.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 28.06.24.
//

import Foundation


class ValidRepeatPinRule: ValidationRule {
    // MARK: - Properties
    private var pin: String = ""
    
    // MARK: - Init
    init(pin: String) {
        self.pin = pin
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        return string == pin ? ValidationResult.valid : ValidationResult(isValid: false, error: .pinMismatch)
    }
}
