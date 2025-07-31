//
//  NotificationTypeDisplayModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.10.23.
//

import Foundation


// MARK: - Notification Type
struct NotificationTypeDisplayModel: Hashable, Identifiable {
    // MARK: - Properties
    let id: String
    let name: String
    var events: [NotificationEventDisplayModel]
    var state: TrippleCheckboxState
    var isMandatory: Bool {
        return events.filter({ !$0.isMandatory }).isEmpty
    }
    var mandatoryEvents: [NotificationEventDisplayModel] {
        return events.filter({ $0.isMandatory })
    }
    var nonMandatoryEvents: [NotificationEventDisplayModel] {
        return events.filter({ !$0.isMandatory })
    }
    
    // MARK: - Helpers
    func calculateState() -> TrippleCheckboxState {
        let checkedEventsCount = events.filter({ $0.isOn }).count
        let uncheckedEventsCount = events.filter({ !$0.isOn}).count
        
        if checkedEventsCount == 0 {
            return .unchecked
        }
        if uncheckedEventsCount == 0 {
            return .checked
        }
        return .semichecked
    }
}
