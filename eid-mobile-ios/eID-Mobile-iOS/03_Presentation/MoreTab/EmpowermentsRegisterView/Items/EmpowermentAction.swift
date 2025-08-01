//
//  EmpowermentMenuOption.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.11.23.
//

import SwiftUI


enum EmpowermentAction: CaseIterable {
    case sign
    case copy
    case withdraw
    case declareDisagreement
    case create
    case eikSearch
}

extension EmpowermentAction {
    var icon: String {
        switch self {
        case .sign:
            ""
        case .copy:
            "icon_copy"
        case .withdraw,
                .declareDisagreement:
            "icon_stop"
        case .create:
            "icon_plus_white"
        case .eikSearch:
            "icon_search"
        }
    }
    
    var title: String {
        switch self {
        case .sign:
            ""
        case .copy:
            "empowerment_menu_option_copy"
        case .withdraw:
            "empowerment_menu_option_withdraw"
        case .declareDisagreement:
            "empowerment_menu_option_declare_disagreement"
        case .create:
            "btn_new_empowerment"
        case .eikSearch:
            "floating_btn_empowerments_eik_search"
        }
    }
    
    var textColor: Color {
        switch self {
        case .sign:
                .clear
        case .copy:
                .textActive
        case .withdraw,
                .declareDisagreement:
                .textError
        case .create:
                .buttonConfirm
        case .eikSearch:
                .buttonDanger
        }
    }
}
