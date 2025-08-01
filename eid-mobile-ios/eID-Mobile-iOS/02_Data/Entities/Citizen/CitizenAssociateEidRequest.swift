//
//  CitizenAssociateEidReuest.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 19.03.25.
//

struct CitizenAssociateEidRequest: Codable {
    struct QueryParams: Codable {
        // MARK: - Properties
        var client_id: String = "eid_ios_app"
    }

    struct BodyParams: Codable {
        // MARK: - Properties
        var signature: String
        var challenge: String
        var certificate: String
        var certificateChain: [String]
    }
    
    
    var queryParams: QueryParams
    var bodyParams: BodyParams
}
