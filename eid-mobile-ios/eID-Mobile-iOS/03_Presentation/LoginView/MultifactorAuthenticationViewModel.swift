//
//  MultifactorAuthenticationViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.04.25.
//

import Foundation
import Alamofire


final class MultifactorAuthenticationViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var otp: ValidInput = ValidInput(value: "",
                                                rules: [NotEmptyRule()])
    @Published var canGenerateNewOTPCode: Bool = false
    @Published var dismissView: Bool = false
    @Published var secondsLeft: Int = 0
    @Published var shouldValidateForm: Bool = false
    var twoFactorResponse: TwoFactorResponse? {
        didSet {
            resetTime()
        }
    }
    private var codeRetryLimit: Int {
        twoFactorResponse?.otpCodeLimit ?? 3
    }
    private var otpRequestTimer: EIDTimer?
    private var codeGenerationCount = 0
    
    // MARK: - API calls
    func submitOTP(onSuccess: @escaping () -> ()) {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let request = VerifyOTPCodeRequest(sessionId: twoFactorResponse?.sessionId ?? "", otp: otp.value)
        AuthRouter.verifyOTPCode(input: request)
            .send(LoginResponse.self) { [weak self] response in
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
                        }
                    }
                case .failure(let error):
                    self?.showError(error: error)
                }
            }
    }
    
    private func showError(error: any Error) {
        showLoading = false
        showError = true
        errorText = error.localizedDescription
    }
    
    func generateOTP() {
        canGenerateNewOTPCode = false
        showLoading = true
        let request = GenerateOTPCodeRequest(sessionId: twoFactorResponse?.sessionId ?? "")
        AuthRouter.generateOTPCode(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(_):
                    self?.codeGenerationCount += 1
                    self?.resetTime()
                    self?.startTimer()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Timer
    func startTimer() {
        otpRequestTimer?.kill()
        otpRequestTimer = EIDTimer(Δt: Double(1), repeats: true, timerFinished: { [weak self] in
            guard let sSelf = self else { return }
            sSelf.secondsLeft -= 1
            
            if sSelf.secondsLeft == 0 {
                sSelf.stopTimer()
                sSelf.dismissView = true
                //                if sSelf.codeGenerationCount == sSelf.codeRetryLimit {
                //                    sSelf.dismissView = true
                //                } else {
                //                    sSelf.canGenerateNewOTPCode = true
                //                }
            }
        })
    }
    
    func stopTimer() {
        otpRequestTimer?.kill()
    }
    
    // MARK: Helpers
    private func resetTime() {
        if let seconds = twoFactorResponse?.ttl {
            secondsLeft = seconds
        }
    }
    
    func validateFields() -> Bool {
        return otp.validation.isValid
    }
}
