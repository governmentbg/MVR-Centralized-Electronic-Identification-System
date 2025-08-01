//
//  BulstatEIK.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 6.06.24.
//

import SwiftUI


struct BulstatEIK: Validation {
    // MARK: - Properties
    var value: String
    // MARK: - Validation override
    func validate() -> ValidationResult {
        return value.isValid(rules: [NotEmptyRule(), ValidEIKBulstatRule()])
    }
}
