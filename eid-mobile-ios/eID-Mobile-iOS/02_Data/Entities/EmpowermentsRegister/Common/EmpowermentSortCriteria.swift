//
//  EmpowermentSortCriteria.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.11.23.
//

import Foundation


enum EmpowermentSortCriteria: String, Codable, CaseIterable {
    case createdOn
    case name
    case providerName
    case serviceName
    case status
    case id
    case authorizer
}

extension EmpowermentSortCriteria {
    static var fromMeCriteria: [EmpowermentSortCriteria] {
        return [.createdOn,
                .name,
                .providerName,
                .serviceName,
                .status]
    }
    
    static var toMeCriteria: [EmpowermentSortCriteria] {
        return [.id,
                .createdOn,
                .authorizer,
                .providerName,
                .serviceName,
                .status]
    }
}

extension EmpowermentSortCriteria {
    var title: String {
        return "empowerment_sort_criteria_\(rawValue)"
    }
}
