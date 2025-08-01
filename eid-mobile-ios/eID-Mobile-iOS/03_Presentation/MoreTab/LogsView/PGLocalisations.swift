//
//  PGLocalisations.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 21.10.24.
//

import Foundation

struct PGSystemLocalisations: Codable {
    // MARK: - Properties
    var logs: Dictionary<String, String>
    var approvalRequestTypes: Dictionary<String, String>
    var errors: Dictionary<String, String>
}

struct PGLocalisedItem: Codable {
    // MARK: - Properties
    var key: String
    var description: String
}
