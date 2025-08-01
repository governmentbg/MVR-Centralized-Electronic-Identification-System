//
//  SetCertificateAliasRequest.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 12.03.25.
//

import Foundation

struct SetCertificateAliasRequest: Codable {
    struct QueryParams: Codable {
        // MARK: - Properties
        var certificateId: String
    }
    
    struct BodyParams: Codable {
        // MARK: - Properties
        var alias: String
    }
    
    var queryParams: QueryParams
    var bodyParams: BodyParams
}
