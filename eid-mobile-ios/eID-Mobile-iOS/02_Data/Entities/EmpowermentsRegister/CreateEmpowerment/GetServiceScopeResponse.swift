//
//  GetServiceScopeResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation


typealias GetServiceScopeResponse = [ServiceScope]


struct ServiceScope: Codable {
    // MARK: - Properties
    var id: String
    var name: String
}
