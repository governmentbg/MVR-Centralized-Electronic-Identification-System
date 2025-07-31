//
//  RepeatedMoreThanOnceRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 6.11.24.
//

import Foundation


class RepeatedMoreThanOnceRule: ValidationRule {
    // MARK: - Properties
    private var array = [String]()
    
    // MARK: - Init
    init(array: [String]) {
        self.array = array
    }
    
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        return array.filter{ $0 == string }.count > 1
        ? ValidationResult(isValid: false, error: .invalidStringRepeatValue)
        : ValidationResult.valid
    }
}
