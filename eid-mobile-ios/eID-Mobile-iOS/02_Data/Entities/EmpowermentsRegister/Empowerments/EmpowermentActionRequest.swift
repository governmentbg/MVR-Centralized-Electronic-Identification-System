//
//  EmpowermentActionRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.12.23.
//

import Foundation


struct EmpowermentActionRequest: Codable {
    // MARK: - Properties
    var empowermentId: String
    var reason: String? = ""
}
