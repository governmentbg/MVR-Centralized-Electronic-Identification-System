//
//  ContinueEIDApplicationIssueViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 9.01.25.
//

import Foundation


final class CompleteEIDApplicationViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var viewState: ViewState = .createCertificate
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var pin: String = ""
    @Published var digitsCount = 6
    private var applicationId: String
    private var enrolledCertificate: EnrollCertificateResponse? = nil
    
    init(applicationId: String) {
        self.applicationId = applicationId
    }
    
    // MARK: - API calls
    func enrollCertificate() {
        guard let csr = generateCSR()
        else {
            showTryAgainError()
            return
        }
        let request = EnrollCertificateEIDRequest(applicationId: applicationId, certificateSigningRequest: csr)
        ElectronicIdentityRouter.enrollCertificateEID(input: request)
            .send(EnrollCertificateResponse.self) { [weak self] response in
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
    
    private func confirmCertificateStorage() {
        showLoading = true
        let request = ConfirmCertificateStorageEIDRequest(applicationId: applicationId)
        ElectronicIdentityRouter.confirmCertificateStorageEID(input: request)
            .send(ConfirmCertificateStorageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let status):
                    guard let status = status,
                          status == .completed
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

extension CompleteEIDApplicationViewModel {
    enum ViewState {
        case createCertificate
        case createPin
    }
}
