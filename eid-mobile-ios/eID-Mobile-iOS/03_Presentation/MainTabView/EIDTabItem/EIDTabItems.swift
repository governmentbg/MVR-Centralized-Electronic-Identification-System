//
//  EIDTabItems.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 10.10.23.
//

import Foundation


enum EIDTabItems: Int, CaseIterable {
    // MARK: - Tabs
    case home = 0
    case electronicIdentity
    case pending
    case more
    
    // MARK: - Title
    var title: String {
        switch self {
        case .home:
            return "tab_title_home"
        case .electronicIdentity:
            return "tab_title_uei"
        case .pending:
            return "tab_title_pending"
        case .more:
            return "tab_title_more"
        }
    }
    
    // MARK: - Icon
    var iconName: String {
        switch self {
        case .home:
            return "icon_home"
        case .electronicIdentity:
            return "icon_uei"
        case .pending:
            return "icon_pending"
        case .more:
            return "icon_more"
        }
    }
}
