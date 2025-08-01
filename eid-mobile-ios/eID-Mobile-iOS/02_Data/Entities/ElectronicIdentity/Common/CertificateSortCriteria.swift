//
//  CertificateSortCriteria.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.04.24.
//

import Foundation


enum CertificateSortCriteria: String, Codable, CaseIterable {
    case status
    case validityFrom
    case eidAdministratorName
    case deviceId
}

extension CertificateSortCriteria {
    var title: String {
        switch self {
        case .validityFrom:
            return "certificate_sort_criteria_create_date"
        case .status:
            return "certificate_sort_criteria_status"
        case .eidAdministratorName:
            return "application_sort_criteria_eid_administrator_name"
        case .deviceId:
            return "application_sort_criteria_device_type"
        }
    }
}
