//
//  GetProvidersResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation


struct GetProvidersPageResponse: Codable {
    // MARK: - Properties
    var pageIndex: Int
    var totalItems: Int
    var data: [ProviderResponse]
}

struct ProviderResponse: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var identificationNumber: String?
    var name: String?
    var isExternal: Bool?
    var status: String?
}
