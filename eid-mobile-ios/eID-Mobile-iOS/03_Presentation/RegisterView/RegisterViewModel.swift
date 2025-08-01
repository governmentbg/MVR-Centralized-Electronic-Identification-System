//
//  RegisterViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.05.24.
//

import Foundation


final class RegisterViewModel: ObservableObject, APICallerViewModel, FieldValidation {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var showInfo: Bool = false
    @Published var shouldValidateForm: Bool = false
    // Input fields
    @Published var firstName: Name = Name(value: "",
                                          isLatin: false,
                                          firstCapitalValidation: true,
                                          isMandatory: true)
    @Published var secondName: Name = Name(value: "",
                                           isLatin: false,
                                           isCompoundName: true) {
        didSet {
            if lastName.otherCompoundName != secondName.value {
                lastName.otherCompoundName = secondName.value
            }
        }
    }
    @Published var lastName: Name = Name(value: "",
                                         isLatin: false,
                                         isCompoundName: true) {
        didSet {
            if secondName.otherCompoundName != lastName.value {
                secondName.otherCompoundName = lastName.value
            }
        }
    }
    @Published var firstNameLatin: Name = Name(value: "",
                                               isLatin: true,
                                               firstCapitalValidation: true,
                                               isMandatory: true)
    @Published var secondNameLatin: Name = Name(value: "",
                                                isLatin: true,
                                                isCompoundName: true) {
        didSet {
            if lastNameLatin.otherCompoundName != secondNameLatin.value {
                lastNameLatin.otherCompoundName = secondNameLatin.value
            }
        }
    }
    @Published var lastNameLatin: Name = Name(value: "",
                                              isLatin: true,
                                              isCompoundName: true) {
        didSet {
            if secondNameLatin.otherCompoundName != lastNameLatin.value {
                secondNameLatin.otherCompoundName = lastNameLatin.value
            }
        }
    }
    @Published var email: Email = Email(value: "")
    @Published var password: Password = Password(value: "",
                                                 isConfirm: false) {
        didSet {
            repeatPassword.comparePassword = password.value
        }
    }
    @Published var repeatPassword: Password = Password(value: "",
                                                       isConfirm: true)
    @Published var phone: Phone = Phone(value: "",
                                        validation: .valid)
    
    // MARK: - API calls
    func register() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let request = RegisterCitizenRequest(firstName: firstName.value,
                                             secondName: secondName.value.isEmpty ? nil : secondName.value,
                                             lastName: lastName.value.isEmpty ? nil : lastName.value,
                                             firstNameLatin: firstNameLatin.value,
                                             secondNameLatin: secondNameLatin.value.isEmpty ? nil : secondNameLatin.value,
                                             lastNameLatin: lastNameLatin.value.isEmpty ? nil : lastNameLatin.value,
                                             email: email.value,
                                             phoneNumber: phone.value.isEmpty ? nil : phone.value,
                                             baseProfilePassword: password.value,
                                             matchingPassword: repeatPassword.value)
        CitizenRouter.register(input: request)
            .send(ServerStatusResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let response):
                    guard let _ = response else { return }
                    self?.showSuccess = true
                    self?.successText = "register_success_title".localized()
                    
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Private methods
    func validateFields() -> Bool {
        if lastName.otherCompoundName?.isEmpty == true {
            lastName.otherCompoundName = secondName.value
        }
        
        if secondName.otherCompoundName?.isEmpty == true {
            secondName.otherCompoundName = lastName.value
        }
        
        if lastNameLatin.otherCompoundName?.isEmpty == true {
            lastNameLatin.otherCompoundName = secondNameLatin.value
        }
        
        if secondNameLatin.otherCompoundName?.isEmpty == true {
            secondNameLatin.otherCompoundName = lastNameLatin.value
        }
        
        let nameIsNotValid = (firstName.validation.isNotValid || secondName.value.isValid(rules: [NotEmptyRule()]).isNotValid)
        && (firstName.validation.isNotValid || lastName.value.isValid(rules: [NotEmptyRule()]).isNotValid)
        
        let latinNameIsNotValid = (firstNameLatin.validation.isNotValid || secondNameLatin.value.isValid(rules: [NotEmptyRule()]).isNotValid)
        && (firstNameLatin.validation.isNotValid || lastNameLatin.value.isValid(rules: [NotEmptyRule()]).isNotValid)
        
        let isNotValid = nameIsNotValid ||
        latinNameIsNotValid ||
        email.validation.isNotValid ||
        password.validation.isNotValid ||
        repeatPassword.validation.isNotValid
        
        return !isNotValid
    }
}

extension RegisterViewModel {
    // MARK: - Fields
    enum Field: Int, CaseIterable {
        case firstName
        case secondName
        case lastName
        case firstNameLatin
        case secondNameLatin
        case lastNameLatin
        case email
        case phone
        case password
        case repeatPassword
    }
    
    func inputFieldValue(field: Field) -> Validation {
        switch field {
        case .firstName:
            firstName
        case .secondName:
            secondName
        case .lastName:
            lastName
        case .firstNameLatin:
            firstNameLatin
        case .secondNameLatin:
            secondNameLatin
        case .lastNameLatin:
            lastNameLatin
        case .email:
            email
        case .phone:
            phone
        case .password:
            password
        case .repeatPassword:
            repeatPassword
        }
    }
}
