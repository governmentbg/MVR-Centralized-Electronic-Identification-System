//
//  EmpowermentsToMeViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.11.23.
//

import Foundation


final class EmpowermentsToMeViewModel: EmpowermentsBaseViewModel {
    // MARK: - API calls
    override func getEmpowerments() {
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
                                             validToDate: validToDate.isEmpty ? nil : validToDate.toDate(withFormats: [.iso8601Date])?.normalizeDate(outputFormat: .iso8601UTC),
                                             showOnlyNoExpiryDate: showOnlyNoExpiryDate ? showOnlyNoExpiryDate : nil)
        showLoading = true
        EmpowermentsRegisterRouter.getEmpowermentsToMe(input: request)
            .send(GetEmpowermentsPageResponse.self) { [weak self] response in
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
