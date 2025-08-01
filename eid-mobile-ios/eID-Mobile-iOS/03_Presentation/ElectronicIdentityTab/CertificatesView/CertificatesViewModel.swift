//
//  CertificatesViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.02.24.
//

import Foundation


final class CertificatesViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var administrators: [EIDAdministrator]
    @Published var devices: [EIDDevice]
    @Published var reasons: [Reason] = []
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showFilter: Bool = false
    @Published var handleAction: Bool = false
    @Published var targetCertificateId: String?
    @Published var targetCertificateDetails: CertificateDetailsResponse?
    @Published var targetCertificateHistory: CertificateHistoryResponse?
    /// API responses
    @Published var certificates: [CertificateResponse] = []
    @Published private var pageIndex = 0
    @Published private var totalItemsCount = 0
    /// Request params
    @Published var sortCriteria: CertificateSortCriteria? = nil//.createDate
    @Published var sortDirection: SortDirection? = nil//.desc
    @Published var id: String = ""
    @Published var serialNumber: String = ""
    @Published var status: CertificateStatus? = nil
    @Published var validityFrom: String = ""
    @Published var validityUntil: String = ""
    @Published var deviceId: String? = nil
    @Published var certificateName: String = ""
    @Published var selectedAdministrator: EIDAdministrator = .default
    var selectedDeviceForPinChange: EIDDevice = .default
    /// Computed
    @Published var isFilterApplied: Bool = false
    private var sortValue: String? {
        guard let criteria = sortCriteria,
              let direction = sortDirection
        else {
            return nil
        }
        return "\(criteria.rawValue),\(direction.rawValue)"
    }
    var sortTitle: String {
        let sortCriteriaText: String = sortCriteria?.title.localized() ?? "sort_by_default_title".localized()
        var sortDirectionText: String = ""
        if let direction = sortDirection {
            sortDirectionText = " (\(direction.title.localized()))"
        }
        return sortCriteriaText + sortDirectionText
    }
    
    // MARK: - Init
    init(administrators: [EIDAdministrator],
         devices: [EIDDevice],
         reasons: [Reason]) {
        self.administrators = administrators
        self.devices = devices
        self.reasons = reasons
    }
    
    // MARK: - API calls
    func getCertificates() {
        showLoading = true
        let request = CertificatesRequest(page: pageIndex,
                                          id: id.isEmpty ? nil : id,
                                          serialNumber: serialNumber.isEmpty ? nil : serialNumber,
                                          status: status,
                                          validityFrom: validityFrom.toDate(withFormats: [.iso8601Date])?.normalizeDate(outputFormat: .iso8601Date),
                                          validityUntil: validityUntil.toDate(withFormats: [.iso8601Date])?.normalizeDate(outputFormat: .iso8601Date),
                                          deviceId: deviceId,
                                          eidAdministratorId: selectedAdministrator.id.isEmpty ? nil : selectedAdministrator.id,
                                          alias: certificateName,
                                          sort: sortValue)
        ElectronicIdentityRouter.getCertificates(input: request)
            .send(CertificatesPageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let certificatesPage):
                    guard let certificatesPage = certificatesPage else { return }
                    if certificatesPage.number == 0 {
                        self?.certificates.removeAll()
                    }
                    self?.totalItemsCount = certificatesPage.totalElements
                    self?.certificates.appendIfMissing(contentsOf: certificatesPage.content)
                    
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func getCertificateDetails(forId certificateId: String, includeHistory: Bool = true) {
        guard !certificateId.isEmpty else { return }
        targetCertificateId = certificateId
        guard let certId = targetCertificateId else { return }
        showLoading = true
        let request = CertificateDetailsRequest(id: certId)
        ElectronicIdentityRouter.getCertificateDetails(input: request)
            .send(CertificateDetailsResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let details):
                    guard let details = details else { return }
                    self?.targetCertificateDetails = details
                    self?.selectedAdministrator = self?.administrators.first(where: { $0.id == details.eidAdministratorId }) ?? .default
                    if includeHistory {
                        self?.getCertificateHistory()
                    } else {
                        self?.handleAction = true
                    }
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func getCertificateHistory() {
        guard let certificateId = targetCertificateId else { return }
        showLoading = true
        let request = CertificateDetailsRequest(id: certificateId)
        ElectronicIdentityRouter.getCertificateHistory(input: request)
            .send(CertificateHistoryResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let history):
                    guard let history = history else { return }
                    self?.targetCertificateHistory = history
                    self?.handleAction = true
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Helpers
    func scrollViewBottomReached() {
        guard certificates.count < totalItemsCount else { return }
        pageIndex += 1
        getCertificates()
    }
    
    func reloadDataFromStart() {
        certificates.removeAll()
        pageIndex = 0
        totalItemsCount = 0
        getCertificates()
        checkFilter()
    }
    
    func checkValidCertificateOwner(id: String) -> Bool {
        let device = getDeviceBy(id: id)
        switch device.id {
        case EIDDevice.mobileDeviceID:
            return CertificateManager.validCertificateOwner()
        case EIDDevice.chipCardID:
            return UserManager.getUser()?.acr == .high
        default:
            return false
        }
    }
    
    func changeDevicePin(id: String) {
        selectedDeviceForPinChange = getDeviceBy(id: id)
        handleAction = true
    }
    
    func getDeviceBy(id: String) -> EIDDevice {
        return devices.first(where: { $0.id == id }) ?? EIDDevice.default
    }
    
    private func checkFilter() {
        isFilterApplied = status != nil || !serialNumber.isEmpty || !validityFrom.isEmpty || !validityUntil.isEmpty || deviceId != nil || !selectedAdministrator.id.isEmpty
    }
}
