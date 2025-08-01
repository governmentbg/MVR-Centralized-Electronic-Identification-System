//
//  CertificateDetailsViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 14.04.24.
//

import Foundation


final class CertificateDetailsViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var administrators: [EIDAdministrator]
    @Published var devices: [EIDDevice]
    @Published var reasons: [Reason] = []
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var viewState: ViewState = .preview
    /// Display data
    @Published var eidentityId: String = ""
    @Published var id: String = ""
    @Published var commonName: String = ""
    @Published var status: CertificateStatus? = nil
    @Published var isExpiring: Bool
    @Published var validityFrom: String = ""
    @Published var validityUntil: String = ""
    @Published var eidAdministratorName: String = ""
    @Published var eidAdministratorOfficeId: String = ""
    @Published var eidAdministratorId: String = ""
    @Published var deviceName: String = ""
    @Published var deviceId: String = ""
    @Published var levelOfAssurance: CertificateLevelOfAssurance? = nil
    @Published private var reasonId: String = ""
    @Published var reason: String = ""
    @Published var serialNumber: String = ""
    @Published var certitifaceAlias: String = ""
    @Published var certificateHistoryItems: [CertificateHistoryItem] = []
    /// Flags
    @Published var selectedAction: CertificateDetailsAction = .noAction
    @Published var applicationDetails: ApplicationDetailsResponse? = nil
    @Published var handleAction: Bool = false
    /// Computed
    var buttonConfig: (title: String, icon: String) {
        switch viewState {
        case .stopCertificate:
            return ("certificate_stop_button_title".localized(), CertificateAction.stop.icon)
        case .resumeCertificate:
            return ("certificate_resume_button_title".localized(), CertificateAction.resume.icon)
        case .revokeCertificate:
            return ("certificate_revoke_button_title".localized(), CertificateAction.revoke.icon)
        default: return ("", "")
        }
    }
    var buttonState: ButtonState {
        switch viewState {
        case .stopCertificate:
            return .warning
        case .resumeCertificate:
            return .success
        case .revokeCertificate:
            return .danger
        default: return .primary
        }
    }
    var shouldShowReasonView: Bool {
        return (status == .stopped || status == .revoked) && !reasonId.isEmpty
    }
    var reasonViewTitle: String {
        switch status {
        case .stopped:
            return "certificate_stopped_reason_title".localized()
        case .revoked:
            return "certificate_revoked_reason_title".localized()
        default: return ""
        }
    }
    var certificateInfo: CertificateDetailsActionModel {
        return CertificateDetailsActionModel(eidAdministratorOfficeId: eidAdministratorOfficeId,
                                             eidAdministratorId: eidAdministratorId,
                                             deviceId: deviceId,
                                             certificateId: id)
    }
    var certificateStatusChangeReasons: [Reason] {
        let stopReasons = reasons.filter({ $0.type == .stop && $0.permittedUser == .publicReason })
        let revokeReasons = reasons.filter({ $0.type == .revoke && $0.permittedUser == .publicReason })
        return viewState == .stopCertificate ? stopReasons : revokeReasons
    }
    
    // MARK: - Init
    init(certificateDetails: CertificateDetailsResponse,
         certificateHistory: CertificateHistoryResponse,
         state: ViewState = .preview,
         administrators: [EIDAdministrator],
         devices: [EIDDevice],
         reasons: [Reason]) {
        self.administrators = administrators
        self.devices = devices
        self.reasons = reasons
        
        eidentityId = certificateDetails.eidentityId ?? ""
        id = certificateDetails.id ?? ""
        commonName = certificateDetails.commonName  ?? ""
        status = certificateDetails.status
        isExpiring = certificateDetails.isExpiring ?? false
        validityFrom = certificateDetails.validityFrom?.toDate()?.normalizeDate(outputFormat: .iso8601DateTime) ?? ""
        validityUntil = certificateDetails.validityUntil?.toDate()?.normalizeDate(outputFormat: .iso8601DateTime) ?? ""
        eidAdministratorName = certificateDetails.eidAdministratorName  ?? ""
        eidAdministratorOfficeId = certificateDetails.eidAdministratorOfficeId  ?? ""
        eidAdministratorId = certificateDetails.eidAdministratorId  ?? ""
        deviceName = devices.first(where: { $0.id == certificateDetails.deviceId })?.name ?? ""
        deviceId = certificateDetails.deviceId  ?? ""
        levelOfAssurance = certificateDetails.levelOfAssurance
        reason = certificateDetails.reasonText ?? ""
        serialNumber = certificateDetails.serialNumber  ?? ""
        certificateHistoryItems = certificateHistory
        certitifaceAlias = certificateDetails.alias ?? ""
        viewState = state
        reasonId = certificateDetails.reasonId ?? ""
        
        if !reasonId.isEmpty && reason.isEmpty == true {
            reason = reasons.filter({ $0.id == reasonId }).first?.description ?? ""
        }
    }
    
    // MARK: - API calls
    func getApplicationDetails(forId applicationId: String) {
        guard !applicationId.isEmpty else { return }
        showLoading = true
        let request = ApplicationDetailsRequest(id: applicationId)
        ElectronicIdentityRouter.getApplicationDetails(input: request)
            .send(ApplicationDetailsResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let details):
                    guard let details = details else { return }
                    self?.applicationDetails = details
                    self?.selectedAction = .goToApplicationDetails
                    self?.handleAction = true
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func goToSetAlias() {
        handleAction = true
        selectedAction = .goToSetAlias
    }
}

// MARK: - Enums
extension CertificateDetailsViewModel {
    enum ViewState {
        case preview
        case stopCertificate
        case resumeCertificate
        case revokeCertificate
        
        var createApplicationFormState: CreateApplicationViewModel.FormState {
            switch self {
            case .preview:
                return .preview
            case .stopCertificate:
                return .stopCertificate
            case .resumeCertificate:
                return .resumeCertificate
            case .revokeCertificate:
                return .revokeCertificate
            }
        }
    }
}

extension CertificateDetailsViewModel {
    enum CertificateDetailsAction {
        case goToSetAlias
        case goToApplicationDetails
        case noAction
    }
}
