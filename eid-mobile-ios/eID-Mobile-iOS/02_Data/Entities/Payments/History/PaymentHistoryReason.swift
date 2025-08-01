//
//  PaymentHistorySubject.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import Foundation

enum PaymentHistoryReason: String, Codable, CaseIterable {
    case stopEid = "STOP_EID"
    case resumeEid = "RESUME_EID"
    case revokeEid = "REVOKE_EID"
    case issueEid = "ISSUE_EID"
    case unknown = ""
}

extension PaymentHistoryReason {
    init(from decoder: any Decoder) throws {
        let container = try decoder.singleValueContainer()
        let rawValue = try container.decode(String.self)
        self = .init(rawValue: rawValue) ?? .unknown
    }
}

extension PaymentHistoryReason {
    var title: String {
        switch self {
        case .stopEid: "payment_history_subject_stop_eid"
        case .resumeEid: "payment_history_subject_resume_eid"
        case .revokeEid: "payment_history_subject_revoke_eid"
        case .issueEid: "payment_history_subject_issue_eid"
        case .unknown:  "status_unknown"
        }
    }
    
    var ordinal: Int {
        switch self {
        case .stopEid: return 0
        case .resumeEid: return 1
        case .revokeEid: return 2
        case .issueEid: return 3
        case .unknown:  return Int.max
        }
    }
}
