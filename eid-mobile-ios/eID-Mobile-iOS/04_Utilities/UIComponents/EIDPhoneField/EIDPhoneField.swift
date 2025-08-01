//
//  EIDPhoneField.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.06.24.
//

import SwiftUI


struct EIDPhoneField: View {
    // MARK: - Properties
    @Binding var phone: Phone
    var title: String
    var isMandatory: Bool = false
    var shouldValidate: Bool
    @State private var isFocused = false
    
    // MARK: - Body
    var body: some View {
        EIDInputField(title: title,
                      text: $phone.value,
                      showError: !$phone.validation.isValid,
                      errorText: $phone.validation.error,
                      shouldValidate: shouldValidate,
                      isMandatory: isMandatory,
                      keyboardType: .phonePad,
                      characterLimit: Constants.Validation.maxPhoneCharacterLimit,
                      focusChanged: { focused in
            isFocused = focused
            focused ? prefillPhoneNumberCode() : clearPhone()
        })
        .onChange(of: phone.value, perform: { _ in
            filterPhone()
        })
    }
    
    // MARK: - Helpers
    private func prefillPhoneNumberCode() {
        if phone.value.isEmpty {
            phone.value = Constants.Validation.bgPhoneCode
        }
    }
    
    private func clearPhone() {
        if phone.value == Constants.Validation.bgPhoneCode {
            phone.value = ""
        }
    }
    
    private func filterPhone() {
        let phoneCodeLength = Constants.Validation.bgPhoneCode.count
        let allowedCharacters = "+0123456789"
        
        let filtered = phone.value.filter { allowedCharacters.contains($0) }
        if filtered != phone.value {
            phone.value = filtered
        }
        
        if isFocused {
            if phone.value.count < phoneCodeLength {
                phone.value = Constants.Validation.bgPhoneCode
            } else if phone.value.count == phoneCodeLength + 1 {
                if phone.value.contains("0") {
                    phone.value = Constants.Validation.bgPhoneCode
                }
            }
        }
    }
}
