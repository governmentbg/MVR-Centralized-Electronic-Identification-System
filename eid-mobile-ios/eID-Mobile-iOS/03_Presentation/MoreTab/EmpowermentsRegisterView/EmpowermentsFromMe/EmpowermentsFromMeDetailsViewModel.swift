//
//  EmpowermentsFromMeDetailsViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.11.23.
//

import Foundation
import Alamofire


final class EmpowermentsFromMeDetailsViewModel: SignatureProviderHelper {
    // MARK: - Properties
    @Published var viewState: ViewState = .preview
    @Published var empowerment: Empowerment
    @Published var reason: String = "Няма да бъде използвано"
    @Published var signatureProviderText: String = ""
    @Published var isInputValid: Bool = false
    /// Indefinite alert properties
    @Published var showWithdrawalAlert: Bool = false
    var screenTitle: String {
        switch viewState {
        case .preview:
            return "empowerment_preview_title".localized()
        case .withdraw:
            return "empowerment_withdraw_title".localized()
        case .sign:
            return "empowerment_signing_title".localized()
        }
    }
    private var statusCheckAttempts: Int = 0
    /// APi responses
    private var detachedSignature: String? = nil
    
    // MARK: - Init
    init(empowerment: Empowerment, viewState: ViewState = .preview) {
        self.empowerment = empowerment
        self.viewState = viewState
        super.init()
        xmlBase64 = empowerment.xmlRepresentation?.toBase64() ?? ""
        fileName = empowerment.number ?? ""
        didSuccessfullyDownloadSignature = { [weak self] signature in
            self?.detachedSignature = signature
            self?.sendDetachedSignature()
        }
    }
    
    // MARK: - API calls
    func withdrawEmpowerment() {
        showLoading = true
        let request = EmpowermentActionRequest(empowermentId: empowerment.id ?? "",
                                               reason: reason)
        EmpowermentsRegisterRouter.withdrawEmpowerment(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success:
                    self?.showSuccess = true
                    self?.successText = "empowerment_withdrawal_success_title".localized()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func sendDetachedSignature() {
        guard let detachedSignature = detachedSignature else {
            showTryAgainError()
            return
        }
        showLoading = true
        let request = EmpowermentSignRequest(empowermentId: empowerment.id ?? "",
                                             signatureProvider: signatureProvider,
                                             detachedSignature: detachedSignature)
        EmpowermentsRegisterRouter.signEmpowerment(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success:
                    self?.showSuccess = true
                    self?.successText = "empowerment_sign_success_title".localized()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Helpers
    func signEmpowerment() {
        guard validateSigningInput(),
        let uid = empowerment.uid else { return }
        startSigningProcess(uid: uid)
    }
    
    func setSignatureProvider(_ provider: SignatureProvider) {
        signatureProvider = provider
        signatureProviderText = provider.title.localized()
        validateSigningInput()
    }
    
    @discardableResult private func validateSigningInput() -> Bool {
        let isValid = signatureProvider == .borica || signatureProvider == .evrotrust
        isInputValid = isValid
        return isValid
    }
    
    private func showTryAgainError() {
        showLoading = false
        showError = true
        errorText = "error_message_try_again".localized()
    }
}


// MARK: - Enums
extension EmpowermentsFromMeDetailsViewModel {
    enum ViewState {
        case preview
        case withdraw
        case sign
    }
}
