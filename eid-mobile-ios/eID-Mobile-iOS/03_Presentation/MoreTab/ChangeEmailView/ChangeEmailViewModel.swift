//
//  ChangeEmailViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.05.24.
//

import Foundation


final class ChangeEmailViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var shouldValidateForm: Bool = false
    /// Input fields
    @Published var email: Email = Email(value: "")
    
    // MARK: - Private methods
    func validateFields() -> Bool {
        return email.validation.isValid
    }
    
    // MARK: - API calls
    func changeEmail() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let request = ChangeCitizenEmailRequest(email: email.value)
        CitizenRouter.changeEmail(input: request)
            .send(ServerStatusResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let response):
                    guard let _ = response else { return }
                    self?.showSuccess = true
                    self?.successText = "change_email_success_title".localized()
                    
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
}
