//
//  PaymentHistoryStatus.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import SwiftUI

enum PaymentHistoryStatus: String, Codable, CaseIterable {
    case pending = "Pending"
    case authorized = "Authorized"
    case ordered = "Ordered"
    case paid = "Paid"
    case expired = "Expired"
    case canceled = "Canceled"
    case suspended = "Suspended"
    case inprocess = "In process"
    case timedout = "TimedOut"
    case unknown = ""
}

extension PaymentHistoryStatus {
    init(from decoder: any Decoder) throws {
        let container = try decoder.singleValueContainer()
        let rawValue = try container.decode(String.self)
        self = .init(rawValue: rawValue) ?? .unknown
    }
}

extension PaymentHistoryStatus {
    var title: String {
        switch self {
        case .pending: "payment_history_status_pending"
        case .authorized: "payment_history_status_authorized"
        case .ordered: "payment_history_status_ordered"
        case .paid: "payment_history_status_paid"
        case .expired: "payment_history_status_expired"
        case .canceled: "payment_history_status_canceled"
        case .suspended: "payment_history_status_suspended"
        case .inprocess: "payment_history_status_inprocess"
        case .timedout: "payment_history_status_timedout"
        case .unknown:  "status_unknown"
        }
    }
    
    var iconName: String {
        switch self {
        case .authorized,
                .paid,
                .inprocess: return "icon_status_active"
        case .expired,
                .canceled,
                .suspended,
                .timedout,
                .unknown: return "icon_cross_red"
        case .pending,
                .ordered: return "icon_clock"
        }
    }
    
    var textColor: Color {
        switch self {
        case .authorized,
                .paid,
                .inprocess: return .buttonConfirm
        case .expired,
                .canceled,
                .suspended,
                .timedout,
                .unknown: return .buttonReject
        case .pending,
                .ordered: return .buttonDanger
        }
    }
    
    var ordinal: Int {
        switch self {
        case .pending: return  0
        case .authorized: return 1
        case .ordered: return 2
        case .paid: return 3
        case .expired: return 4
        case .canceled: return 5
        case .suspended: return 6
        case .inprocess: return 7
        case .timedout: return 8
        case .unknown: return Int.max
        }
    }
}
