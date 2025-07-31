//
//  ApplicationsViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.02.24.
//

import Foundation


final class ApplicationsViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var administrators: [EIDAdministrator]
    @Published var devices: [EIDDevice]
    @Published var reasons: [Reason] = []
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showFilter: Bool = false
    @Published var handleAction: Bool = false
    @Published var targetApplicationDetails: ApplicationDetailsResponse?
    /// API responses
    @Published var applications: [ApplicationResponse] = []
    @Published private var pageIndex = 0
    @Published private var totalItemsCount = 0
    /// Request params
    @Published var sortCriteria: ApplicationSortCriteria? = nil//.createDate
    @Published var sortDirection: SortDirection? = nil//.desc
    @Published var status: ApplicationStatus? = nil
    @Published var id: String = ""
    @Published var applicationNumber = ""
    @Published var createDateText: String = ""
    @Published var deviceId: String? = nil
    @Published var applicationType: EIDApplicationType? = nil
    @Published var selectedAdministrator: EIDAdministrator = .default
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
    func getApplications() {
        showLoading = true
        let startDate = createDateText.isEmpty ? nil : createDateText.toDate(withFormats: [.iso8601Date])
        let request = ApplicationsRequest(page: pageIndex,
                                          status: status,
                                          id: id.isEmpty ? nil : id,
                                          applicationNumber: applicationNumber.isEmpty ? nil : applicationNumber,
                                          deviceId: deviceId,
                                          createdDateFrom: startDate?.normalizeDate(outputFormat: .iso8601UTC),
                                          createdDateTo: startDate?.endOfDay.normalizeDate(outputFormat: .iso8601UTC),
                                          applicationType: applicationType,
                                          eidAdministratorId: selectedAdministrator.id.isEmpty ? nil : selectedAdministrator.id,
                                          sort: sortValue)
        ElectronicIdentityRouter.getApplications(input: request)
            .send(ApplicationsPageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let applicationsPage):
                    guard let applicationsPage = applicationsPage else { return }
                    if applicationsPage.number == 0 {
                        self?.applications.removeAll()
                    }
                    self?.totalItemsCount = applicationsPage.totalElements
                    self?.applications.appendIfMissing(contentsOf: applicationsPage.content)
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
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
                    self?.targetApplicationDetails = details
                    self?.handleAction = true
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Helpers
    func scrollViewBottomReached() {
        guard applications.count < totalItemsCount else { return }
        pageIndex += 1
        getApplications()
    }
    
    func reloadDataFromStart() {
        applications.removeAll()
        pageIndex = 0
        totalItemsCount = 0
        getApplications()
        checkFilter()
    }
    
    private func checkFilter() {
        isFilterApplied = status != nil
        || !id.isEmpty
        || !applicationNumber.isEmpty
        || !createDateText.isEmpty
        || deviceId != nil
        || applicationType != nil
        || !selectedAdministrator.id.isEmpty
    }
}
