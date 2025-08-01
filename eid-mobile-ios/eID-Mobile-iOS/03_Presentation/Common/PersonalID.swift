//
//  PersonalID.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 27.11.23.
//

import SwiftUI


struct PersonalID: Validation {
    // MARK: - Properties
    var idType: IdentifierType {
        didSet {
            idTypeText = idType.title.localized()
        }
    }
    var idTypeText: String
    var id: String
    var name: Name
    var visible: Bool
    var currentIds: [String]?
    var shouldValidateEmpowerment: Bool = false
    
    // MARK: - Validation fields
    let fieldId = UUID()
    let nameFieldId = UUID()
    
    // MARK: - Validation override
    func validate() -> ValidationResult {
        var idTypeValidation: ValidationResult = .valid
        
        switch idType {
        case .lnch:
            idTypeValidation = id.isValid(rules: [NotEmptyRule(), ValidLNCHRule()])
        case .egn:
            idTypeValidation = id.isValid(rules: [NotEmptyRule(), ValidEGNRule()])
        default:
            break
        }
        
        if idTypeValidation.isValid {
            if shouldValidateEmpowerment && id == UserManager.getUser()?.citizenIdentifier {
                idTypeValidation = ValidationResult(isValid: false, error: .invalidSelfEmpowerment)
            } else {
                idTypeValidation = validateIds()
            }
        }
        
        return idTypeValidation
    }
    
    // MARK: - Validation helpers
    private func validateIds() -> ValidationResult {
        if currentIds?.isEmpty == false {
            return id.isValid(rules: [RepeatedMoreThanOnceRule(array: currentIds ?? [])])
        }
        return .valid
    }
}

/// MARK: Validation helpers
extension PersonalID {
    var isNotValidField: Bool {
        id.isEmpty || validation.isNotValid || name.validation.isNotValid
    }
    
    var idIsNotValid: Bool {
        id.isEmpty || validation.isNotValid
    }
    
    var nameIsNotValid: Bool {
        name.validation.isNotValid
    }
}

struct PersonalIDField {
    // MARK: - Properties
    @Binding var personalId: PersonalID
    @State var idTypeField: AnyView
    @State var idField: EIDInputField
    @State var nameField: EIDNameField?
}
