//
//  ApplicationDetailsViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.03.24.
//

import Foundation
import Alamofire


final class ApplicationDetailsViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var administrators: [EIDAdministrator]
    @Published var devices: [EIDDevice]
    @Published var reasons: [Reason] = []
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    /// Display data
    @Published var applicationId: String = ""
    @Published var applicationType: EIDApplicationType? = nil
    @Published var applicationNumber: String = ""
    @Published var createDateString: String = ""
    @Published var administratorName: String = ""
    @Published var administratorOffice: String = ""
    @Published var deviceName: String = ""
    @Published var status: ApplicationStatus? = nil
    @Published var reason: String? = nil
    @Published var certificateNumber: String? = nil
    @Published var name: String = ""
    @Published var email: String = ""
    @Published var phoneNumber: String = ""
    @Published var identityType: String = ""
    @Published var identityNumber: String = ""
    @Published var identityIssueDate: String = ""
    @Published var identityValidityToDate: String = ""
    @Published var paymentAccessCode: String = ""
    @Published var displayContinueButton: Bool = false
    @Published var selectedAction: ApplicationDetailsAction = .noAction
    /// Request params
    @Published private var certificateId: String? = nil
    /// Flags
    @Published var certificateDetails: CertificateDetailsResponse? = nil
    @Published var certificateHistory: CertificateHistoryResponse? = nil
    @Published var handleAction: Bool = false
    
    // MARK: - Init
    init(applicationDetails details: ApplicationDetailsResponse,
         administrators: [EIDAdministrator],
         devices: [EIDDevice],
         reasons: [Reason]) {
        self.administrators = administrators
        self.devices = devices
        self.reasons = reasons
        
        applicationId = details.id
        applicationType = details.applicationType
        applicationNumber = details.applicationNumber ?? "N/A"
        createDateString = details.createDate.toDate()?.toLocalTime().normalizeDate(outputFormat: .iso8601DateTime) ?? "N/A"
        administratorName = details.eidAdministratorName
        administratorOffice = details.eidAdministratorOfficeName
        deviceName = devices.first(where: { $0.id == details.deviceId })?.name ?? ""
        status = details.status
        reason = details.reasonText
        certificateId = details.certificateId
        certificateNumber = details.serialNumber
        name = [details.firstName, details.secondName, details.lastName].fullName
        email = details.email ?? "N/A"
        phoneNumber = details.phoneNumber ?? "phone_number_not_specified".localized()
        identityType = details.identityType ?? "N/A"
        identityNumber = details.identityNumber ?? "N/A"
        identityIssueDate = details.identityIssueDate?.toDate()?.normalizeDate(outputFormat: .iso8601Date) ?? "N/A"
        identityValidityToDate = details.identityValidityToDate?.toDate()?.normalizeDate(outputFormat: .iso8601Date) ?? "N/A"
        paymentAccessCode = details.paymentAccessCode ?? ""
        /// TODO: HIGH ACR REMOVED FOR TESTING PURPOSES. REMOVE BEFORE RELEASE!!!
        displayContinueButton = (status == .approved || status == .paid) && administratorOffice == "ONLINE" /*&& UserManager.getUser()?.acr == .high && details.administratorOfficeId == Constants.Administrator.Office.onlineOfficeId*/
        
        if let reasonId = details.reasonId {
            reason = reasons.filter({ $0.id == reasonId }).first?.description ?? details.reasonText
        }
    }
    
    // MARK: - API calls
    func getCertificateDetails() {
        guard let certificateId = certificateId else { return }
        showLoading = true
        let request = CertificateDetailsRequest(id: certificateId)
        ElectronicIdentityRouter.getCertificateDetails(input: request)
            .send(CertificateDetailsResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let details):
                    guard let details = details else { return }
                    self?.certificateDetails = details
                    self?.getCertificateHistory()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func getCertificateHistory() {
        guard let certificateId = certificateId else { return }
        showLoading = true
        let request = CertificateDetailsRequest(id: certificateId)
        ElectronicIdentityRouter.getCertificateHistory(input: request)
            .send(CertificateHistoryResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let history):
                    guard let history = history else { return }
                    self?.certificateHistory = history
                    self?.handleAction = true
                    self?.selectedAction = .goToCertificateDetails
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func completeApplicationStatusChange() {
        showLoading = true
        let request = CompleteCertificateStatusChangeRequest(applicationId: applicationId)
        ElectronicIdentityRouter.completeCertificateStatusChange(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(_):
                    self?.showSuccess = true
                    self?.successText = self?.applicationType?.successMessage.localized() ?? ""
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
}

extension ApplicationDetailsViewModel {
    enum ApplicationDetailsAction {
        case goToCertificateDetails
        case continueIssue
        case noAction
    }
}
