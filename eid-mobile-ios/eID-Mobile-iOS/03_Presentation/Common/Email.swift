//
//  Email.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 6.06.24.
//

import SwiftUI


struct Email: Validation {
    // MARK: - Properties
    var value: String
    // MARK: - Validation override
    func validate() -> ValidationResult {
        return value.isValid(rules: [NotEmptyRule(), ValidEmailRule(), ValidMinimumLengthRule(min: 5)])
    }
}
