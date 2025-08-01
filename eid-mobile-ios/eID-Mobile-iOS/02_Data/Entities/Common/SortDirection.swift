//
//  SortDirection.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.11.23.
//

import Foundation


enum SortDirection: String, Codable {
    case asc
    case desc
}

extension SortDirection {
    var title: String {
        return "sort_title_\(rawValue)"
    }
}
