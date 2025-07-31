//
//  PaymentHistorySortCriteria.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import Foundation

enum PaymentHistorySortCriteria: String, Codable, CaseIterable {
    case createdOn
    case subject
    case paymentDate
    case validUntil
    case status
    case amount
    case lastSync
}

extension PaymentHistorySortCriteria {
    var title: String {
        switch self {
        case .createdOn:
            return "payment_history_sort_criteria_created_on"
        case .subject:
            return "payment_history_sort_criteria_subject"
        case .paymentDate:
            return "payment_history_sort_criteria_payment_date"
        case .validUntil:
            return "payment_history_sort_criteria_valid_until"
        case .status:
            return "payment_history_sort_criteria_status"
        case .amount:
            return "payment_history_sort_criteria_amount"
        case .lastSync:
            return "payment_history_sort_criteria_last_sync"
        }
    }
}
