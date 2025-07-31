//
//  Phone.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.06.24.
//

import SwiftUI


struct Phone: Validation {
    // MARK: - Properties
    var value: String {
        didSet {
            validation = (value.isEmpty && !isMadatory) ? .valid : value.isValid(rules: [ValidLengthRule(length: Constants.Validation.maxPhoneCharacterLimit)])
        }
    }
    var validation: ValidationResult
    var isMadatory: Bool = false
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        return (value.isEmpty && !isMadatory) ? .valid : value.isValid(rules: [ValidLengthRule(length: Constants.Validation.maxPhoneCharacterLimit)])
    }
}
