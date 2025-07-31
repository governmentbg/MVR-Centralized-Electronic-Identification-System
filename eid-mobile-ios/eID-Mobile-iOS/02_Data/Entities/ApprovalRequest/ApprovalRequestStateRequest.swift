//
//  ApprovalRequestStateRequest.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 30.07.24.
//

import Foundation


struct ApprovalRequestStateRequest: Codable {
    struct QueryParams: Codable {
        // MARK: - Properties
        var approvalRequestId: String
    }
    
    struct BodyParams: Codable {
        // MARK: - Properties
        var signedChallenge: SignedChallenge?
        var approvalRequestStatus: ApprovalRequestStatus
        var cliendId: String = "eid_ios_app"
    }
    
    var queryParams: QueryParams
    var bodyParams: BodyParams
}

enum ApprovalRequestStatus: String, Codable {
    case succeed = "SUCCEED"
    case cancelled = "CANCELLED"
}
