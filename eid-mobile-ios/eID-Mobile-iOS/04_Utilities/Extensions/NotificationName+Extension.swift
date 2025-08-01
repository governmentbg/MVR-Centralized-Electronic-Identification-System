//
//  NotificationName+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.02.24.
//

import Foundation


extension Notification.Name {
    /// Server Response
    static let unauthorizedRequest = Notification.Name("unauthorizedRequest")
    static let serverFailure = Notification.Name("serverFailure")
    static let inactivityLogout = Notification.Name("inactivityLogout")
    static let nfcSheetWasDismissed = Notification.Name("nfcSheetWasDismissed")
}
