//
//  MainLoginViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.09.24.
//

import Foundation
import UIKit


final class MainLoginViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showLanguageAlert: Bool = false
    @Published var languageAlertTitle: String = "language_settings_alert_text".localized()
    @Published var currentEnv: AppEnvironment = AppConfiguration.currentEnvironment()
    @Published var showLoginWithPINView: Bool = false
    @Published var pin: String = ""
    @Published var showTwoFactorAuthView: Bool = false
    @Published var handleAction: Bool = false
    @Published var selectedAction: MainLoginAction = .noAction
    @Published var didFinishCardLoginSuccessfully: Bool = false
    @Published var didReceiveNFCCardSessionEnd: Bool = false
    var twoFactorResponse: TwoFactorResponse?
    var languageAlertButtons: [AlertButton] {
        return [
            AlertButton(title: "btn_cancel".localized()),
            AlertButton(title: "more_settings_title".localized(), action: { [weak self] in
                self?.openLanguageSettings()
            })
        ]
    }
    var buttonTitle: String {
        switch (UserManager.hasDevicePin, UserManager.biometricsAvailable) {
        case (true, false):
            "device_pin_login_button_title".localized()
        case (true, true):
            "biometrics_login_button_title".localized()
        default:
            "btn_login_with_password".localized()
        }
    }
    
    // MARK: - API calls
    func getAuthToken(onSuccess: @escaping () -> (), onFailure: @escaping () -> ()) {
        guard let credentials = UserManager.credentials else {
            return
        }
        showLoginWithPINView = false
        showLoading = true
        let request = BasicLoginRequest(email: credentials.email,
                                        password: credentials.password)
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
                        onSuccess()
                    } onFailure: { error in
                        self?.showError(error: error)
                        onFailure()
                    }
                } else if let twoFactorResponse = response?.twoFactorResponse {
                    self?.twoFactorResponse = twoFactorResponse
                    self?.showTwoFactorAuthView = true
                    self?.selectedAction = .goToTwoFactorView
                    self?.handleAction = true
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
    
    // MARK: - Methods
    func loginAction(onBiometricsSuccess: @escaping () -> ()) {
        if UserManager.hasDevicePin {
            if UserManager.biometricsAvailable {
                BiometricProvider.authenticate { [weak self] success, authenticationError in
                    DispatchQueue.main.async {
                        if success {
                            self?.getAuthToken(onSuccess: {
                                onBiometricsSuccess()
                            }, onFailure: {})
                        } else {
                            if authenticationError == .fallback {
                                self?.showLoginWithPINView = true
                            } else {
                                self?.showError = true
                                self?.errorText = authenticationError?.description ?? ""
                            }
                        }
                    }
                }
            } else {
                showLoginWithPINView = true
            }
        } else {
            selectedAction = .goToLoginView
            handleAction = true
        }
    }
    
    func setEnv(_ env: AppEnvironment) {
        AppConfiguration.setEnvironment(env: env)
        currentEnv = AppConfiguration.currentEnvironment()
    }
    
    private func openLanguageSettings() {
        guard let settingsUrl = URL(string: UIApplication.openSettingsURLString) else { return }
        if UIApplication.shared.canOpenURL(settingsUrl) {
            UIApplication.shared.open(settingsUrl)
        }
    }
}


extension MainLoginViewModel {
    enum MainLoginAction {
        case goToLoginView
        case goToTwoFactorView
        case goTologinWithMobileEidView
        case goTologinWithCardView
        case goToRegisterView
        case noAction
    }
}
