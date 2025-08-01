//
//  ApplicationAction.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.12.24.
//

import SwiftUI


enum ApplicationAction {
    case pay
    case noAction
}

extension ApplicationAction {
    static func action(status: ApplicationStatus) -> ApplicationAction {
        switch status {
        case .pendingPayment:
            return .pay
        default:
            return .noAction
        }
    }
    
    var icon: String {
        switch self {
        case .pay:
            return "creditcard.fill"
        case .noAction: return ""
        }
    }
    
    var textColor: Color {
        switch self {
        case .pay:
            return .buttonDefault
        case .noAction: return .clear
        }
    }
    
    var buttonTitle: String {
        switch self {
        case .pay:
            return "btn_payment"
        case .noAction: return ""
        }
    }
}
