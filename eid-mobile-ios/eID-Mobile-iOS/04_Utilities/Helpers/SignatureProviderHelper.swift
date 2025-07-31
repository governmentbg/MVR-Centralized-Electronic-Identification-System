//
//  SignatureProviderHelper.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 11.06.24.
//

import Foundation
import SwiftUI
import Alamofire


class SignatureProviderHelper: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showSigningLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    var signatureProvider: SignatureProvider = .evrotrust
    var xmlBase64: String = ""
    var fileName: String = ""
    var didSuccessfullyDownloadSignature: ((String) -> ())? = nil
    // MARK: - Private Properties
    private var statusCheckAttempts: Int = 0
    private var documentTransactionId: String? = nil
    private var shoulContinueStatusPing = true
    private var uid: String?
    
    // MARK: - Public API
    func startSigningProcess(uid: String) {
        self.uid = uid
        switch signatureProvider {
        case .evrotrust:
            checkEurotrustUser()
        case .borica:
            checkBoricaUser()
        default:
            break
        }
    }
    
    // MARK: - Private API
    private func startStatusChecks(signatureProvider: SignatureProvider) {
        if InactivityHelper.type != .signing {
            InactivityHelper.setType(newType: .signing)
            shoulContinueStatusPing = true
        }
        Timer.scheduledTimer(withTimeInterval: 5, repeats: true) { [weak self] timer in
            guard let sSelf = self else { return }
            guard sSelf.shoulContinueStatusPing else {
                sSelf.handleStatusCheckResponseLoading()
                timer.invalidate()
                return
            }
            guard sSelf.statusCheckAttempts < Constants.Signing.limit else {
                sSelf.handleStatusCheckResponseLoading()
                sSelf.showTryAgainError()
                timer.invalidate()
                return
            }
            sSelf.statusCheckAttempts += 1
            sSelf.checkStatus(signatureProvider: signatureProvider)
        }.fire()
    }
    
    private func checkStatus(signatureProvider: SignatureProvider) {
        switch signatureProvider {
        case .evrotrust:
            checkEurotrustStatus()
        case .borica:
            checkBoricaStatus()
        default: break
        }
    }
    
    private func handleStatusCheckResponseLoading() {
        InactivityHelper.setType(newType: .normal)
        shoulContinueStatusPing = false
        showSigningLoading = false
        showLoading = true
    }
    
    private func showSignLoading() {
        showLoading = false
        showSigningLoading = true
    }
    
    /// Eurotrust flow
    private func checkEurotrustUser() {
        guard let uid = uid else { return }
        let checkUserRequest = CheckSingingUserRequest(uid: uid)
        showLoading = true
        SigningRouter.checkEurotrustUser(input: checkUserRequest)
            .send(EurotrustCheckUserResponse.self) { [weak self] response in
                switch response {
                case .success(let user):
                    guard let user = user,
                          user.isReadyToSign
                    else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.signWithEurotrust()
                case .failure(let error):
                    self?.showLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func signWithEurotrust() {
        guard let uid = uid else { return }
        showLoading = true
        let expiryDate = Date.now.addDay(n: 1).normalizeDate(outputFormat: .iso8601UTC)
        let document = EurotrustDocument(content: xmlBase64,
                                         fileName: "\(fileName).xml")
        let request = EurotrustSignRequest(dateExpire: expiryDate,
                                           documents: [document],
                                           uid: uid)
        SigningRouter.signWithEurotrust(input: request)
            .send(EurotrustSignResponse.self) { [weak self] response in
                switch response {
                case .success(let signResponse):
                    guard let signResponse = signResponse,
                          let documentTransaction = signResponse.transactions.first,
                          !documentTransaction.transactionId.isEmpty
                    else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.documentTransactionId = documentTransaction.transactionId
                    self?.statusCheckAttempts = 0
                    self?.startStatusChecks(signatureProvider: .evrotrust)
                case .failure(let error):
                    self?.showLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func checkEurotrustStatus() {
        guard let transactionId = documentTransactionId else {
            showTryAgainError()
            return
        }
        showSignLoading()
        let request = EurotrustTransactionRequest(transactionId: transactionId)
        SigningRouter.getEurotrustStatus(input: request)
            .send(EurotrustStatusResponse.self) { [weak self] response in
                switch response {
                case .success(let eurotrustStatus):
                    guard let eurotrustStatus = eurotrustStatus else { return }
                    guard eurotrustStatus.status != .rejected else {
                        self?.showTryAgainError()
                        self?.statusCheckAttempts = Constants.Signing.limit
                        return
                    }
                    if eurotrustStatus.status == .signed,
                       eurotrustStatus.isProcessing == false {
                        InactivityHelper.setType(newType: .normal)
                        self?.handleStatusCheckResponseLoading()
                        self?.downloadEurotrustSignature()
                    }
                case .failure(let error):
                    self?.showSigningLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func downloadEurotrustSignature() {
        guard let transactionId = documentTransactionId else {
            showTryAgainError()
            return
        }
        let request = EurotrustTransactionRequest(transactionId: transactionId)
        SigningRouter.downloadEurotrustSignature(input: request)
            .send(EurotrustSignatureResponse.self) { [weak self] response in
                switch response {
                case .success(let eurotrustSignature):
                    guard let signature = eurotrustSignature?.first else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.didSuccessfullyDownloadSignature?(signature.content)
                case .failure(let error):
                    self?.showLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    /// Borica flow
    private func checkBoricaUser() {
        guard let uid = uid else { return }
        let checkUserRequest = CheckSingingUserRequest(uid: uid)
        showLoading = true
        SigningRouter.checkBoricaUser(input: checkUserRequest)
            .send(BoricaCheckUserResponse.self) { [weak self] response in
                switch response {
                case .success(let user):
                    guard let user = user,
                          user.responseCode == "OK"
                    else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.signWithBorica()
                case .failure(let error):
                    self?.showLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func signWithBorica() {
        guard let uid = uid else { return }
        showLoading = true
        let document = BoricaDocument(data: xmlBase64,
                                      fileName: "\(fileName).xml")
        let request = BoricaSignRequest(contents: [document], uid: uid)
        SigningRouter.signWithBorica(input: request)
            .send(BoricaSignResponse.self) { [weak self] response in
                switch response {
                case .success(let signResponse):
                    guard let documentTransaction = signResponse?.data,
                          !documentTransaction.callbackId.isEmpty
                    else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.documentTransactionId = documentTransaction.callbackId
                    self?.statusCheckAttempts = 0
                    self?.startStatusChecks(signatureProvider: .borica)
                case .failure(let error):
                    self?.showLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func checkBoricaStatus() {
        guard let transactionId = documentTransactionId else {
            showTryAgainError()
            return
        }
        showSignLoading()
        let request = BoricaTransactionRequest(transactionId: transactionId)
        SigningRouter.getBoricaStatus(input: request)
            .send(BoricaStatusResponse.self) { [weak self] response in
                switch response {
                case .success(let boricaStatus):
                    guard let boricaStatus = boricaStatus,
                          let boricaSignature = boricaStatus.data.signatures.first
                    else {
                        self?.showTryAgainError()
                        return
                    }
                    if boricaStatus.responseCode == .completed {
                        switch boricaSignature.status {
                        case .signed:
                            self?.documentTransactionId = boricaSignature.signature
                            self?.handleStatusCheckResponseLoading()
                            self?.downloadBoricaSignature()
                            return
                        case .rejected:
                            self?.handleStatusCheckResponseLoading()
                            self?.showTryAgainError()
                            return
                        default: return
                        }
                    }
                case .failure(let error):
                    self?.showSigningLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func downloadBoricaSignature() {
        guard let transactionId = documentTransactionId else {
            showTryAgainError()
            return
        }
        let request = BoricaTransactionRequest(transactionId: transactionId)
        SigningRouter.downloadBoricaSignature(input: request)
            .send(BoricaSignatureResponse.self) { [weak self] response in
                switch response {
                case .success(let boricaSignature):
                    guard let signature = boricaSignature?.content else {
                        self?.showTryAgainError()
                        return
                    }
                    self?.didSuccessfullyDownloadSignature?(signature)
                case .failure(let error):
                    self?.showLoading = false
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func showTryAgainError() {
        showLoading = false
        showError = true
        errorText = "error_message_try_again".localized()
    }
}
