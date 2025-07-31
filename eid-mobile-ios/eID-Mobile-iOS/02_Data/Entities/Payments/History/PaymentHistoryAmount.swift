//
//  PaymentHistoryAmount.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import Foundation

enum PaymentHistoryAmount: Hashable {
    case below(value: Int)
    case between(value: ClosedRange<Int>)
    case above(value: Int)
}

extension PaymentHistoryAmount {
    var title: String {
        switch self {
        case .below(let value):
            return String(format: "payment_history_amount_below".localized(), value)
        case .between(let value):
            return String(format: "payment_history_amount_between".localized(), value.lowerBound, value.upperBound)
        case .above(let value):
            return String(format: "payment_history_amount_over".localized(), value)
        }
    }
}
