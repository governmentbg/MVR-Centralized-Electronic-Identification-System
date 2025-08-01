//
//  EIDNameField.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 25.06.24.
//

import SwiftUI


struct EIDNameField: View {
    // MARK: - Properties
    var title: String
    @Binding var name: Name
    var isMandatory: Bool
    var shouldValidate: Bool
    var autocapitalization: InputFieldCapitalization?
    var characterLimit: Int?
    var hint: String = ""
    var rightIcon: EIDInputField.RightIconType = .none
    var rightIconAction: () -> Void = {}
    
    // MARK: - Body
    var body: some View {
        EIDInputField(title: title,
                      hint: hint,
                      text: $name.value,
                      showError: !$name.validation.isValid,
                      errorText: $name.validation.error,
                      shouldValidate: shouldValidate,
                      rightIcon: rightIcon,
                      rightIconAction: rightIconAction,
                      isMandatory: isMandatory,
                      autocapitalization: autocapitalization ?? .never,
                      characterLimit: characterLimit ?? 1000,
                      focusChanged: { focused in
            trimName(isFocised: focused)
        })
    }
    
    // MARK: - Helpers
    private func trimName(isFocised: Bool) {
        if !isFocised {
            name.value = name.value.condensedWhitespace
        }
    }
}
