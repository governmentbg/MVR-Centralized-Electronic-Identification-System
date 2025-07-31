//
//  SetApprovalRequestStateResponse.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.08.24.
//

import Foundation


struct SetApprovalRequestStateResponse: Codable {
    // MARK: - Properties
    var wildcardSubtype: Bool
    var wildcardType: Bool
    var type: String
}
