//
//  ChangePasswordViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.05.24.
//

import Foundation


final class ChangePasswordViewModel: ObservableObject, APICallerViewModel, FieldValidation {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var shouldValidateForm: Bool = false
    // Input fields
    @Published var oldPassword: ValidInput = ValidInput(value: "",
                                                        rules: [NotEmptyRule(),
                                                                ValidMinimumLengthRule(min: 3)])
    @Published var newPassword: Password = Password(value: "",
                                                    isConfirm: false) {
        didSet {
            confirmNewPassword.comparePassword = newPassword.value
        }
    }
    @Published var confirmNewPassword: Password = Password(value: "",
                                                           isConfirm: true)
    
    // MARK: - Private methods
    func validateFields() -> Bool {
        let isNotValid = oldPassword.validation.isNotValid ||
        newPassword.validation.isNotValid ||
        confirmNewPassword.validation.isNotValid
        return !isNotValid
    }
    
    // MARK: - API calls
    func changePassword() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let request = ChangeCitizenPasswordRequest(oldPassword: oldPassword.value,
                                                   newPassword: newPassword.value,
                                                   confirmPassword: confirmNewPassword.value)
        CitizenRouter.changePassword(input: request)
            .send(ServerStatusResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let response):
                    guard let _ = response else { return }
                    self?.showSuccess = true
                    self?.successText = "change_password_success_title".localized()
                    
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
}


extension ChangePasswordViewModel {
    // MARK: - Fields
    enum Field: Int, CaseIterable {
        case oldPassword
        case newPassword
        case confirmNewPassword
    }
    
    func inputFieldValue(field: Field) -> Validation {
        switch field {
        case .oldPassword:
            oldPassword
        case .newPassword:
            newPassword
        case .confirmNewPassword:
            confirmNewPassword
        }
    }
}
