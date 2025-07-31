//
//  FieldValidation.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 28.05.25.
//

import Foundation


protocol FieldValidation {
    associatedtype Field: CaseIterable
    var shouldValidateForm: Bool { get set }
    func inputFieldValue(field: Field) -> Validation
    func validateFields() -> Bool
}

extension FieldValidation {
    var firstErrorField: Field? {
        var firstErrorField: Field?
        for field in Field.allCases {
            if inputFieldValue(field: field).validation.isNotValid {
                firstErrorField = field
                break
            }
        }
        return firstErrorField
    }
}
