//
//  GetServiceScopeRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation


struct GetServiceScopeRequest: Codable {
    // MARK: - Properties
    var serviceId: String
    var includeDeleted: Bool = false
}
