//
//  ForgottenPasswordViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.05.24.
//

import Foundation


final class ForgottenPasswordViewModel: ObservableObject, APICallerViewModel, FieldValidation {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var shouldValidateForm: Bool = false
    /// Input fields
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
    @Published var email: Email = Email(value: "")
    
    // MARK: - API calls
    func submit() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let request = ForgottenPasswordCheckRequest(firstName: firstName.value,
                                                    secondName: secondName.value,
                                                    lastName: lastName.value,
                                                    email: email.value)
        CitizenRouter.forgottenPasswordCheck(input: request)
            .send(ServerStatusResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let response):
                    guard let _ = response else { return }
                    self?.showSuccess = true
                    self?.successText = "forgotten_password_success_title".localized()
                    
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
        
        let nameIsNotValid = (firstName.validation.isNotValid || secondName.value.isValid(rules: [NotEmptyRule()]).isNotValid)
        && (firstName.validation.isNotValid || lastName.value.isValid(rules: [NotEmptyRule()]).isNotValid)
        
        let isNotValid = nameIsNotValid || email.validation.isNotValid
        return !isNotValid
    }
}


extension ForgottenPasswordViewModel {
    // MARK: - Fields
    enum Field: Int, CaseIterable {
        case firstName
        case secondName
        case lastName
        case email
    }
    
    func inputFieldValue(field: Field) -> Validation {
        switch field {
        case .firstName:
            firstName
        case .secondName:
            secondName
        case .lastName:
            lastName
        case .email:
            email
        }
    }
}
