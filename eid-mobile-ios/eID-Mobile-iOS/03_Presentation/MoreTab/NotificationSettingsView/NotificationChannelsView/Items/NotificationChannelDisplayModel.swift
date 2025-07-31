//
//  NotificationChannelDisplayModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.10.23.
//

import Foundation


struct NotificationChannelDisplayModel: Hashable {
    // MARK: - Properties
    var id: String
    var name: String
    var description: String
    var isSelected: Bool = false
    var isMandatory: Bool = false
}
