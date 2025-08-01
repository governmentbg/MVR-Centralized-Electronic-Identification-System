//
//  EmpowermentsFromMeViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.11.23.
//

import Foundation

final class EmpowermentsFromMeViewModel: EmpowermentsBaseViewModel {
    @Published var viewState: EmpowermentDirection = .fromMe
    
    var screenTitle: String {
        return viewState == .fromMe
        ? "empowerments_from_me_screen_title".localized()
        : String(format: "empowerment_eik_search_screen_title".localized(), eik)
    }
    
    // MARK: - API calls
    override func getEmpowerments() {
        if eik.isEmpty == false && onBehalfOf != .legalEntity {
            onBehalfOf = .legalEntity
        }
        let request = GetEmpowermentsRequest(eik: eik.isEmpty ? nil : eik,
                                             pageIndex: pageIndex,
                                             sortby: sortCriteria,
                                             sortDirection: sortDirection,
                                             number: number.isEmpty ? nil : number,
                                             status: status,
                                             onBehalfOf: onBehalfOf == .empty ? nil : onBehalfOf,
                                             authorizer: authorizer.isEmpty ? nil : authorizer,
                                             providerName: providerName.isEmpty ? nil : providerName,
                                             serviceName: serviceName.isEmpty ? nil : serviceName,
                                             empoweredUids: empoweredUids.filter(
                                                { $0.uid != "" && $0.uid?.isEmpty == false }
                                             ).isEmpty ? nil : empoweredUids,
                                             validToDate: validToDate.isEmpty ? nil : validToDate.toDate(withFormats: [.iso8601Date])?.endOfDay.normalizeDate(outputFormat: .iso8601UTC),
                                             showOnlyNoExpiryDate: showOnlyNoExpiryDate ? showOnlyNoExpiryDate : nil)
        showLoading = true
        let router = viewState == .fromMe
        ? EmpowermentsRegisterRouter.getEmpowermentsFromMe(input: request)
        : EmpowermentsRegisterRouter.getEmpowermentsFromMeEIK(input: request)
        router.send(GetEmpowermentsPageResponse.self) { [weak self] response in
            self?.showLoading = false
            switch response {
            case .success(let empowermentPage):
                guard let empowermentPage = empowermentPage else { return }
                if empowermentPage.pageIndex == 1 {
                    self?.empowerments.removeAll()
                }
                self?.totalItemsCount = empowermentPage.totalItems
                self?.empowerments.appendIfMissing(contentsOf: empowermentPage.data)
            case .failure(let error):
                self?.showError = true
                self?.errorText = error.localizedDescription
            }
        }
    }
}
