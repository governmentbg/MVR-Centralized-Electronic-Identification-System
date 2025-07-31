//
//  EmpowermentsBaseViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 22.05.24.
//

import Foundation


class EmpowermentsBaseViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showFilter: Bool = false
    @Published var selectedAction: EmpowermentAction? = nil
    @Published var targetEmpowerment: Empowerment?
    @Published var handleAction: Bool = false
    /// API responses
    @Published var empowerments: [Empowerment] = []
    /// Request params
    @Published var sortCriteria: EmpowermentSortCriteria? = nil
    @Published var sortDirection: SortDirection? = nil
    @Published var number: String = ""
    @Published var status: EmpowermentStatus? = nil
    @Published var onBehalfOf: EmpowermentOnBehalfOf? = nil
    @Published var authorizer: String = ""
    @Published var providerName: String = ""
    @Published var serviceName: String = ""
    @Published var empoweredUids: [UserIdentifier] = []
    @Published var validToDate: String = ""
    @Published var eik: String = ""
    @Published var showOnlyNoExpiryDate: Bool = false
    @Published var pageIndex = 1
    @Published var totalItemsCount = 0
    var sortTitle: String {
        let sortCriteriaText: String = sortCriteria?.title.localized() ?? "sort_by_default_title".localized()
        var sortDirectionText: String = ""
        if let direction = sortDirection {
            sortDirectionText = " (\(direction.title.localized()))"
        }
        return sortCriteriaText + sortDirectionText
    }
    @Published var isFilterApplied: Bool = false
    
    // MARK: - API calls
    func getEmpowerments() { }
    
    // MARK: - Helpers
    func scrollViewBottomReached() {
        guard empowerments.count < totalItemsCount else { return }
        pageIndex += 1
        getEmpowerments()
    }
    
    func reloadDataFromStart() {
        empowerments.removeAll()
        pageIndex = 1
        totalItemsCount = 0
        getEmpowerments()
        checkFilter()
    }
    
    func handleEmpowermentAction(_ action: EmpowermentAction, for empowerment: Empowerment?) {
        selectedAction = action
        handleAction = true
        targetEmpowerment = empowerment
    }
    
    private func checkFilter() {
        isFilterApplied = !number.isEmpty || status != nil || onBehalfOf != .empty || !authorizer.isEmpty || !providerName.isEmpty || !serviceName.isEmpty || showOnlyNoExpiryDate || !validToDate.isEmpty || !empoweredUids.isEmpty || !eik.isEmpty
    }
}

// MARK: - Enums
extension EmpowermentsBaseViewModel {
    enum ViewState {
        case fromMe
        case toMe
        case fromMeEIK
    }
}
