//
//  NotificationEventDisplayModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.11.23.
//

import Foundation


// MARK: - Notification Event
struct NotificationEventDisplayModel: Hashable, Identifiable {
    // MARK: - Properties
    let id: String
    let name: String
    var isOn: Bool = false
    var isMandatory: Bool = false
}
