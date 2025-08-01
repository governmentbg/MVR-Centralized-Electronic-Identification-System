//
//  Alias.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 12.03.25.
//

struct Alias: Validation {
    // MARK: - Properties
    var value: String
    var original: String
    var isMandatory: Bool = false
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        value.isValid(rules: [NotEmptyRule(), NotMatchRule(original: original)])
    }
    
    // MARK: - Private Properties
}
