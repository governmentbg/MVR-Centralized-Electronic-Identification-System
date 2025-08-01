//
//  ApplicationSortCriteria.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.03.24.
//

import Foundation


enum ApplicationSortCriteria: String, Codable, CaseIterable {
    case status
    case createDate
    case eidAdministratorName
    case deviceType
    case applicationType
}


extension ApplicationSortCriteria {
    var title: String {
        switch self {
        case .status:
            return "application_sort_criteria_status"
        case .createDate:
            return "application_sort_criteria_create_date"
        case .eidAdministratorName:
            return "application_sort_criteria_eid_administrator_name"
        case .deviceType:
            return "application_sort_criteria_device_type"
        case .applicationType:
            return "application_sort_criteria_application_type"
        }
    }
}
