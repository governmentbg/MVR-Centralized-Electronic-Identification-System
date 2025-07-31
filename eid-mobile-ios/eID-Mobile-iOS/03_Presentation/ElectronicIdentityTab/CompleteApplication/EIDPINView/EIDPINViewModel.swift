//
//  EIDPINViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 21.05.24.
//

import SwiftUI


@MainActor
final class EIDPINViewModel: ObservableObject, @preconcurrency APICallerViewModel {
    // MARK: - Properties
    @Published var state: ViewState
    @Published var pin: String = ""
    @Published var can: String = ""
    @Published var digitsCount: Int = 6
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showInfo: Bool = false
    /// Card authentication
    private var eidReader = EIDReader(cardKeyType: .rsa)
    private var challenge: String = ""
    private var signedChallenge: String = ""
    private var cardCertificate: String = ""
    private var certificateChain = [String]()
    var canProceed: Bool {
        return switch state {
        case .loginWithCardCan:
            [pin, can].allSatisfy { $0.count == digitsCount }
        default:
            pin.count == digitsCount
        }
    }
    private var pinFirstEntry: String = ""
    private var pinSecondEntry: String = ""
    private var pinCodeMaxAttempts = 3
    var onPINCreate: ((Bool, String) -> Void)? = nil
    var onPINAuth: ((Bool) -> Void)? = nil
    var onAuthRequest: ((CertificateLoginRequest) -> Void)? = nil
    
    // MARK: - Init
    init(state: ViewState, pin: String = "",
         digitsCount: Int = 6,
         onPINCreate: ( (Bool, String) -> Void)? = nil,
         onPINAuth: ( (Bool) -> Void)? = nil,
         onAuthRequest: ( (CertificateLoginRequest) -> Void)? = nil) {
        self.state = state
        self.pin = pin
        self.digitsCount = digitsCount
        self.onPINCreate = onPINCreate
        self.onPINAuth = onPINAuth
        self.onAuthRequest = onAuthRequest
    }
    
    // MARK: - Methods
    func submitPIN() {
        switch state {
        case .createPIN:
            pinFirstEntry = pin
            state = .verifyPIN
            pin = ""
        case .createDevicePIN:
            let validationResult = pin.isValid(rules: [ValidDevicePINRule()])
            if validationResult.isValid {
                pinFirstEntry = pin
                state = .verifyDevicePIN
            } else {
                showError = true
                errorText = validationResult.validationError?.description.localized() ?? ""
            }
            pin = ""
        case .loginWithDevicePIN:
            if pin != UserManager.credentials?.pin {
                pinCodeMaxAttempts -= 1
                if pinCodeMaxAttempts > 0 {
                    pin = ""
                    showError = true
                    errorText = String(format: "wrong_device_pin_error".localized(), pinCodeMaxAttempts)
                } else {
                    onPINCreate?(false, "")
                }
            } else {
                onPINCreate?(true, pin)
            }
        case .verifyPIN:
            pinSecondEntry = pin
            onPINCreate?(pinFirstEntry == pinSecondEntry, pinFirstEntry == pinSecondEntry ? pin : "")
        case .verifyDevicePIN:
            if pin != pinFirstEntry {
                pin = ""
                showError = true
                errorText = "validation_error_pin_mismatch".localized()
            } else {
                pinSecondEntry = pin
                onPINCreate?(pinFirstEntry == pinSecondEntry, pinFirstEntry == pinSecondEntry ? pin : "")
            }
        case .loginWithCard, .loginWithCardCan:
            getLoginChallenge()
        case .loginWithMobileEID:
            guard CertificateManager.validateCertificatePin(pin) else {
                clearPIN()
                showError = true
                errorText = "error_wrong_pin".localized()
                onPINAuth?(false)
                return
            }
            getLoginChallenge()
        case .approveAuthRequest:
            getLoginChallenge()
        }
    }
    
    func clearPIN() {
        pin = ""
        can = ""
        if state == .verifyPIN {
            state = .createPIN
        }
    }
    
    // MARK: - API calls
    private func getLoginChallenge() {
        showLoading = true
        let request = GetChallengeRequest()
        AuthRouter.getChallenge(input: request).send(GetChallengeResponse.self) {[weak self] response in
            self?.showLoading = false
            switch response {
            case .success(let challengeResponse):
                guard let challenge = challengeResponse?.challenge else { return }
                self?.challenge = challenge
                self?.signedChallenge = ""
                self?.signChallenge()
            case .failure(let error):
                self?.showError(error: error)
            }
        }
    }
    
    private func certificateLogin() {
        guard !pin.isEmpty,
              !challenge.isEmpty,
              !signedChallenge.isEmpty,
              !cardCertificate.isEmpty
        else {
            return
        }
        showLoading = true
        let request = CertificateLoginRequest(signedChallenge: SignedChallenge(signature: signedChallenge,
                                                                               challenge: challenge,
                                                                               certificate: cardCertificate,
                                                                               certificateChain: certificateChain))
        AuthRouter.certificateLogin(input: request).send(AuthResponse.self) {[weak self] response in
            self?.clearPIN()
            switch response {
            case .success(let authResponse):
                guard let token = authResponse?.accessToken else { return }
                StorageManager.keychain.save(key: .authToken,
                                             value: token)
                InactivityHelper.startTimer()
                PendingAuthRequestsHelper.checkPendingAuthRequest()
                VerifyLoginRequestHelper.verifyLogin {
                    self?.showLoading = false
                    self?.onPINAuth?(true)
                } onFailure: { error in
                    self?.showError(error: error)
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
    
    private func signChallenge() {
        switch state {
        case .loginWithCard, .loginWithCardCan:
            signChallengeWithCard()
        case .loginWithMobileEID:
            signChallengeWithLocalCertificate()
        case .approveAuthRequest(let method):
            method == .cardEID ? signChallengeWithCard() : signChallengeWithLocalCertificate()
        default:
            break
        }
    }
    
    // MARK: - Local certificate operations
    private func signChallengeWithLocalCertificate() {
        guard !challenge.isEmpty,
              let storedCertificate = CertificateManager.getCertificate(),
              let storedCertificateChain = CertificateManager.getCertificateChain(),
              let storedSignature = SecKeyManager.getSignature(for: challenge)
        else { return }
        signedChallenge = storedSignature
        cardCertificate = storedCertificate
        certificateChain = storedCertificateChain
        completeSignAction()
    }
    
    // MARK: - Card operations
    private func signChallengeWithCard() {
        guard !pin.isEmpty,
              !challenge.isEmpty
        else { return }
        Task { @MainActor in
            do {
                let response = try await eidReader.signChallenge(pin: pin,
                                                                 challenge: challenge, can: can.isEmpty ? nil : can)
                signedChallenge = response.signedChallenge
                cardCertificate = response.certificate
                certificateChain = response.certificateChain
                completeSignAction()
            } catch let error {
                if let cardError = error as? NFCEIDCardReaderError, case NFCEIDCardReaderError.CardSuspended = cardError {
                    clearPIN()
                    state = .loginWithCardCan
                }
            }
        }
    }
    
    private func completeSignAction() {
        switch state {
        case .approveAuthRequest(_):
            let request = CertificateLoginRequest(signedChallenge: SignedChallenge(signature: signedChallenge,
                                                                                   challenge: challenge,
                                                                                   certificate: cardCertificate,
                                                                                   certificateChain: certificateChain))
            onAuthRequest?(request)
        default:
            certificateLogin()
        }
    }
}


extension EIDPINViewModel {
    enum ViewState: Equatable {
        case createPIN
        case verifyPIN
        case loginWithCard
        case loginWithCardCan
        case loginWithMobileEID
        case approveAuthRequest(PendingRequestSignMethod)
        case createDevicePIN
        case verifyDevicePIN
        case loginWithDevicePIN
        
        var screenTitle: String {
            switch self {
            case .createPIN,
                    .verifyPIN:
                return "complete_application_title".localized()
            case .loginWithCard, .loginWithCardCan,
                    .loginWithMobileEID, .loginWithDevicePIN:
                return "btn_login".localized()
            case .approveAuthRequest:
                return "eid_authenticate_title".localized()
            case .createDevicePIN, .verifyDevicePIN:
                return "create_device_pin_title".localized()
            }
        }
        
        var inputTitle: String {
            switch self {
            case .createPIN, .createDevicePIN:
                return "eid_create_pin_title".localized()
            case .verifyPIN, .verifyDevicePIN:
                return "eid_verify_pin_title".localized()
            case .loginWithCard:
                return "eid_card_use_pin_title".localized()
            case .loginWithCardCan:
                return "eid_card_use_can_title".localized()
            case .loginWithDevicePIN:
                return "eid_use_pin_title".localized()
            case .loginWithMobileEID:
                return "eid_use_mobile_pin_title".localized()
            case .approveAuthRequest:
                return "eid_use_pin_title".localized()
            }
        }
    }
    
}
