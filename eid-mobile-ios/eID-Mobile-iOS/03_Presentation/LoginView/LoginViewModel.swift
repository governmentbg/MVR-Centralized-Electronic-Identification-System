//
//  LoginViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.10.23.
//

import Foundation


final class LoginViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showTwoFactorAuthView: Bool = false
    @Published var shouldValidateForm: Bool = false
    @Published var email: Email = Email(value: "")
    @Published var password: ValidInput = ValidInput(value: "",
                                                     rules: [NotEmptyRule(),
                                                             ValidMinimumLengthRule(min: 3)])
    var twoFactorResponse: TwoFactorResponse?
    
    // MARK: - API calls
    func getAuthToken(onSuccess: @escaping () -> (), onFailure: @escaping () -> ()) {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let request = BasicLoginRequest(email: email.value,
                                        password: password.value)
        AuthRouter.basicLogin(input: request).send(LoginResponse.self) {[weak self] response in
            switch response {
            case .success(let response):
                if let authResponse = response?.authResponse {
                    let token = authResponse.accessToken
                    StorageManager.keychain.save(key: .authToken,
                                                 value: token)
                    InactivityHelper.startTimer()
                    PendingAuthRequestsHelper.checkPendingAuthRequest()
                    VerifyLoginRequestHelper.verifyLogin {
                        self?.showLoading = false
                        self?.saveCredentialsToKeychain()
                        onSuccess()
                    } onFailure: { error in
                        self?.showError(error: error)
                        onFailure()
                    }
                } else if let twoFactorResponse = response?.twoFactorResponse {
                    self?.twoFactorResponse = twoFactorResponse
                    self?.showTwoFactorAuthView = true
                }
            case .failure(let error):
                self?.showError(error: error)
                onFailure()
            }
        }
    }
    
    private func showError(error: any Error) {
        showLoading = false
        showError = true
        errorText = error.localizedDescription
    }
    
    // MARK: - Helpers
    func clearFields() {
#if DEBUG
        let testUser = TestUsers.teddy
#else
        let testUser = TestUsers.none
#endif
        email.value = testUser.username
        password.value = testUser.password
        showLoading = false
    }
    
    func validateFields() -> Bool {
        return email.validation.isValid && password.validation.isValid
    }
    
    // MARK: - Private methods
    private func saveCredentialsToKeychain() {
        let credentials = UserCredentials(email: email.value, password: password.value)
        UserManager.saveUserCredentials(credentials: credentials)
    }
    
    /// 401 handling
    func observeUnauthorized() {
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(handleUnauthorizedRequest),
                                               name: .unauthorizedRequest,
                                               object: nil)
    }
    
    @objc private func handleUnauthorizedRequest() {
        showLoading = false
        showError = true
        errorText = "error_invalid_credentials".localized()
    }
}
