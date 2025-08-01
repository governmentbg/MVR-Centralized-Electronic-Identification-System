//
//  CompleteApplicationViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.05.24.
//

import Foundation
import UIKit


final class CompleteApplicationViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var viewState: ViewState = .scanQR
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var pin: String = ""
    @Published var digitsCount = 6
    private var otpCode: String = ""
    private var enrolledCertificate: EnrollCertificateResponse? = nil
    
    // MARK: - API calls
    private func signApplication() {
        guard !otpCode.isEmpty,
              let instanceId = UserManager.getMobileAppInstanceId(),
              let firebaseId = UserManager.getFirebaseId()
        else {
            showTryAgainError()
            return
        }
        showLoading = true
        let request = SignApplicationRequest(otpCode: otpCode,
                                             mobileApplicationInstanceId: instanceId,
                                             firebaseId: firebaseId)
        ElectronicIdentityRouter.signApplicationBaseProfile(input: request)
            .send(SignApplicationResponse.self) { [weak self] response in
                self?.showLoading = false
                switch  response {
                case .success(let status):
                    guard status == .signed else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.enrollCertificate()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func enrollCertificate() {
        guard !otpCode.isEmpty,
              let csr = generateCSR()
        else {
            showTryAgainError()
            return
        }
        showLoading = true
        let request = EnrollCertificateBaseProfileRequest(otpCode: otpCode,
                                                          certificateSigningRequest: csr)
        ElectronicIdentityRouter.enrollCertificateBaseProfile(input: request)
            .send(EnrollCertificateResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let enrolledCertificate):
                    guard let certificate = enrolledCertificate else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.enrolledCertificate = certificate
                    self?.viewState = .createPin
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func confirmCertificateStorage() {
        guard !otpCode.isEmpty else {
            showTryAgainError()
            return
        }
        showLoading = true
        let request = ConfirmCertificateStorageBaseProfileRequest(otpCode: otpCode)
        ElectronicIdentityRouter.confirmCertificateStorageBaseProfile(input: request)
            .send(ConfirmCertificateStorageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let status):
                    guard let status = status,
                          status == .certificateStored
                    else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.showSuccessMessage()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Public helpers
    func processScannedCode(_ code: String) {
        guard !code.isEmpty else {
            showTryAgainError()
            return
        }
        otpCode = code
        signApplication()
    }
    
    func storePinAndCertificate(pin: String) {
        guard pin.count == digitsCount,
              let certificateResponse = enrolledCertificate
        else {
            showTryAgainError()
            return
        }
        guard CertificateManager.storeCertificate(cert: certificateResponse.certificate,
                                                  certChain: certificateResponse.certificateChain,
                                                  pin: pin)
        else {
            showTryAgainError()
            return
        }
        confirmCertificateStorage()
    }
    
    // MARK: - Private helpers
    private func generateCSR() -> String? {
        var csr: String? = nil
        do {
            csr = try SecKeyManager.generateCSR(useNewKey: true)
        } catch {
            return nil
        }
        return csr?.toBase64()
    }
    
    private func showTryAgainError() {
        showLoading = false
        showError = true
        errorText = "error_message_try_again".localized()
    }
    
    private func showSuccessMessage() {
        showLoading = false
        showSuccess = true
        successText = "complete_application_success_title".localized()
    }
}

extension CompleteApplicationViewModel {
    enum ViewState {
        case scanQR
        case createPin
    }
}
