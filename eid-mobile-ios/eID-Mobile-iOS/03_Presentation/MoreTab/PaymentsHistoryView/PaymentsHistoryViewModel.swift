//
//  PaymentsHistoryViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import Foundation

final class PaymentsHistoryViewModel: ObservableObject, APICallerViewModel {
    
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showFilter: Bool = false
    @Published var sortCriteria: PaymentHistorySortCriteria? = nil//.createDate
    @Published var sortDirection: SortDirection? = nil//.desc
    /// API responses
    private var payments = [PaymentHistoryResponse]()
    private var sortedPaymnets = [PaymentHistoryResponse]()
    @Published var paymentNumber: String = ""
    @Published var status: PaymentHistoryStatus? = nil
    @Published var createdOn: String = ""
    @Published var reason: PaymentHistoryReason? = nil
    @Published var amount: PaymentHistoryAmount? = nil
    @Published var paymentDate: String = ""
    @Published var validUntil: String = ""
    @Published var filteredPayments: [PaymentHistoryResponse] = []
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
    
    // MARK: - API calls
    func getPayments() {
        showLoading = true
        PaymentsRouter.getHistory
            .send([PaymentHistoryResponse].self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let payments):
                    self?.payments.appendIfMissing(contentsOf: payments ?? [])
                    self?.applySortingAndFilter()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func applySortingAndFilter() {
        appySorting()
        applyFilterData()
        checkFilter()
    }
    
    private func appySorting() {
        sortedPaymnets = switch sortDirection {
        case .asc:
            switch sortCriteria {
            case .createdOn:
                payments.sorted { $0.createdOn?.toDate()?.timeIntervalSince1970 ?? 0 < $1.createdOn?.toDate()?.timeIntervalSince1970 ?? 0 }
            case .subject:
                payments.sorted { ($0.reason?.ordinal ?? 0) < ($1.reason?.ordinal ?? 0) }
            case .paymentDate:
                payments.sorted { $0.paymentDate?.toDate()?.timeIntervalSince1970 ?? 0 < $1.paymentDate?.toDate()?.timeIntervalSince1970 ?? 0 }
            case .validUntil:
                payments.sorted { $0.paymentDeadline?.toDate()?.timeIntervalSince1970 ?? 0 < $1.paymentDeadline?.toDate()?.timeIntervalSince1970 ?? 0 }
            case .status:
                payments.sorted { ($0.status?.ordinal ?? 0) < ($1.status?.ordinal ?? 0) }
            case .amount:
                payments.sorted { ($0.payment?.first?.fee ?? 0) < ($1.payment?.first?.fee ?? 0) }
            case .lastSync:
                payments.sorted { $0.lastSync?.toDate()?.timeIntervalSince1970 ?? 0 < $1.lastSync?.toDate()?.timeIntervalSince1970 ?? 0 }
            case nil:
                payments
            }
        case .desc:
            switch sortCriteria {
            case .createdOn:
                payments.sorted { $0.createdOn?.toDate()?.timeIntervalSince1970 ?? 0 > $1.createdOn?.toDate()?.timeIntervalSince1970 ?? 0 }
            case .subject:
                payments.sorted { ($0.reason?.ordinal ?? 0) > ($1.reason?.ordinal ?? 0) }
            case .paymentDate:
                payments.sorted { $0.paymentDate?.toDate()?.timeIntervalSince1970 ?? 0 > $1.paymentDate?.toDate()?.timeIntervalSince1970 ?? 0 }
            case .validUntil:
                payments.sorted { $0.paymentDeadline?.toDate()?.timeIntervalSince1970 ?? 0 > $1.paymentDeadline?.toDate()?.timeIntervalSince1970 ?? 0 }
            case .status:
                payments.sorted { ($0.status?.ordinal ?? 0) > ($1.status?.ordinal ?? 0) }
            case .amount:
                payments.sorted { ($0.payment?.first?.fee ?? 0) > ($1.payment?.first?.fee ?? 0) }
            case .lastSync:
                payments.sorted { $0.lastSync?.toDate()?.timeIntervalSince1970 ?? 0 > $1.lastSync?.toDate()?.timeIntervalSince1970 ?? 0 }
            case nil:
                payments
            }
        case nil:
            payments
        }
    }
    
    private func applyFilterData() {
        filteredPayments = paymentNumber.isEmpty ? sortedPaymnets : sortedPaymnets.filter {
            $0.ePaymentId?.contains(paymentNumber) ?? false
        }
        
        filteredPayments = status == nil ? filteredPayments : filteredPayments.filter {
            $0.status == status
        }
        
        filteredPayments = createdOn.isEmpty ? filteredPayments : filteredPayments.filter {
            $0.createdOn?.toDate()?.startOfDay.timeIntervalSince1970 ?? 0 == createdOn.toDate(withFormats: [.iso8601Date])?.timeIntervalSince1970 ?? 0
        }
        
        filteredPayments = reason == nil ? filteredPayments : filteredPayments.filter {
            $0.reason == reason
        }
        
        filteredPayments = switch amount {
        case .below(let value):
            filteredPayments.filter {
                Int($0.payment?.first?.fee ?? 0) < value
            }
        case .between(let value):
            filteredPayments.filter {
                value.contains(Int($0.payment?.first?.fee ?? 0))
            }
        case .above(let value):
            filteredPayments.filter {
                Int($0.payment?.first?.fee ?? 0) > value
            }
        case nil:
            filteredPayments
        }
        
        
        filteredPayments = paymentDate.isEmpty ? filteredPayments : filteredPayments.filter {
            $0.paymentDate?.toDate()?.startOfDay.timeIntervalSince1970 ?? 0 == paymentDate.toDate(withFormats: [.iso8601Date])?.timeIntervalSince1970 ?? 0
        }
        
        filteredPayments = validUntil.isEmpty ? filteredPayments : filteredPayments.filter {
            $0.paymentDeadline?.toDate()?.timeIntervalSince1970 ?? 0 <= validUntil.toDate(withFormats: [.iso8601Date])?.endOfDay.timeIntervalSince1970 ?? 0
        }
    }
    
    private func checkFilter() {
        isFilterApplied = status != nil || !paymentNumber.isEmpty || !validUntil.isEmpty || !paymentDate.isEmpty || reason != nil || !createdOn.isEmpty || amount != nil
    }
    
}
