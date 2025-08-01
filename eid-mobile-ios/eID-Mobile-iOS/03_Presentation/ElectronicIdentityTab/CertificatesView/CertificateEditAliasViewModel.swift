//
//  CertificateEditAliasViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 12.03.25.
//

import SwiftUI

final class CertificateEditAliasViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var shouldValidateForm: Bool = false
    /// Input fields
    @Published var alias = Alias(value: "", original: "")
    @Published private var certificateId: String = ""
    
    init(certificateId: String, alias: Alias) {
        self.certificateId = certificateId
        self.alias = alias
    }
    
    // MARK: - Private methods
    func validateFields() -> Bool {
        return alias.validation.isValid
    }
    
    // MARK: - API calls
    func setAlias() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let queryParams = SetCertificateAliasRequest.QueryParams(certificateId: certificateId)
        let bodyParams = SetCertificateAliasRequest.BodyParams(alias: alias.value)
        let request = SetCertificateAliasRequest(queryParams: queryParams, bodyParams: bodyParams)
        ElectronicIdentityRouter.setCertificateAlias(input: request)
            .send(ServerStatusResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let response):
                    guard let _ = response else { return }
                    self?.showSuccess = true
                    self?.successText = "edit_certificate_alias_success_title".localized()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
}
