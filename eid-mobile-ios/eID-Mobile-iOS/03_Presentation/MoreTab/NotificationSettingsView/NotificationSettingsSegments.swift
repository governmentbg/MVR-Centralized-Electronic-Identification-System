//
//  NotificationSettingsSegments.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation


enum NotificationSettingsSegments: Int, CaseIterable {
    // MARK: - Tabs
    case channels = 0
    case notificationTypes
    
    // MARK: - Title
    var title: String {
        switch self {
        case .channels:
            return "Канали"
        case .notificationTypes:
            return "Известия"
        }
    }
    
    // MARK: - Icon
    var iconName: String {
        switch self {
        case .channels:
            return "icon_channels"
        case .notificationTypes:
            return "icon_notification_types"
        }
    }
}
