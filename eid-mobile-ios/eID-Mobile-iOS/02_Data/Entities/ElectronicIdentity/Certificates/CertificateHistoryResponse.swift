//
//  CertificateHistoryResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 19.09.24.
//

import Foundation


typealias CertificateHistoryResponse = [CertificateHistoryItem]


struct CertificateHistoryItem: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var createdDateTime: String
    var validityUntil: String
    var validityFrom: String
    var status: CertificateStatus?
    var applicationId: String?
    var applicationNumber: String?
    var modifiedDateTime: String?
    var reasonId: String?
    var reasonText: String?
}


extension CertificateHistoryItem {
    // MARK: - Helpers
    var hasApplicationNumber: Bool {
        switch status {
        case .active,
                .created,
                .revoked,
                .stopped:
            return applicationNumber != nil && applicationNumber != ""
        default:
            return false
        }
    }
    
    func getReason(fromReasons reasons: [Reason] = []) -> String? {
        if let reasonText = reasonText {
            return reasonText
        }
        guard let id = reasonId,
              !reasons.isEmpty
        else { return nil }
        let reasonLocalizations = reasons.filter({ $0.id == id })
        return reasonLocalizations.first(where: { $0.language == LanguageManager.preferredLanguage?.rawValue })?.description
    }
}
