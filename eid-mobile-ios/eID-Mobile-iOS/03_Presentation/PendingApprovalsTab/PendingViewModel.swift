//
//  PendingViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 30.07.24.
//

import Foundation


final class PendingViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var showPINView: Bool = false
    @Published var showSignMethodPicker: Bool = false
    private var inactivityTimer: EIDTimer?
    /// API Data
    @Published var requests: [ApprovalRequest] = []
    var selectedRequest: ApprovalRequest?
    var selectedSignMethod: PendingRequestSignMethod = .cardEID
    
    // MARK: - API calls
    func getRequests() {
        showLoading = true
        ApprovalRequestRouter.getApprovalRequests.send(ApprovalRequestResponse.self) { [weak self] response in
            self?.showLoading = false
            switch response {
            case .success(let response):
                self?.requests = response?.requests ?? []
            case .failure(let error):
                self?.showError = true
                self?.errorText = error.localizedDescription
            }
        }
    }
    
    func setRequestState(state: ApprovalRequestStatus, request: CertificateLoginRequest? = nil) {
        guard let selectedRequest = selectedRequest else { return }
        showLoading = true
        let query = ApprovalRequestStateRequest.QueryParams(approvalRequestId: selectedRequest.id ?? "")
        let challenge = state == .cancelled ? nil : request?.signedChallenge
        let body = ApprovalRequestStateRequest.BodyParams(signedChallenge: challenge, approvalRequestStatus: state)
        let request = ApprovalRequestStateRequest(queryParams: query, bodyParams: body)
        
        ApprovalRequestRouter.setApprovalRequestState(input: request).send(SetApprovalRequestStateResponse.self) { [weak self] response in
            self?.showLoading = false
            switch response {
            case .success(let response):
                guard let _ = response else { return }
                self?.showSuccess = true
                self?.successText = state == .cancelled ? "approval_request_cancelled_title".localized() : "approval_request_success_title".localized()
            case .failure(let error):
                self?.showError = true
                self?.errorText = error.localizedDescription
            }
        }
    }
    
    // MARK: - Helpers
    func getRequestTitle(request: ApprovalRequest) -> String {
        return GlobalLocalisations.approvalRequestTypes.first(where: { $0.key == (request.requestFrom?.type ?? "") })?.description ?? request.requestFrom?.type ?? ""
    }
    
    // MARK: - Timer
    func startTimer() {
        if inactivityTimer != nil {
            inactivityTimer?.kill()
        }
        
        inactivityTimer = EIDTimer(Δt: Double(60), timerFinished: { [weak self] in
            self?.getRequests()
        })
    }
    
    func stopTimer() {
        inactivityTimer?.kill()
    }
}

// MARK: - Enums
enum PendingRequestSignMethod: String, CaseIterable {
    case cardEID, mobileEID
    
    var title: String {
        switch self {
        case .cardEID:
            return "identity_type_title_identity_card".localized()
        case .mobileEID:
            return "request_sign_method_mobile_eid_title".localized()
        }
    }
}
