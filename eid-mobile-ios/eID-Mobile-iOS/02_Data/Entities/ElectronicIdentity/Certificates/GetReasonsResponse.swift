//
//  GetReasonsResponse.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 24.04.24.
//

import Foundation


typealias GetReasonsResponse =  [ReasonType]


extension GetReasonsResponse {
    // MARK: - Properties
    var stopReasons: [Reason] {
        return filterReasonsFor(key: TypeEnum.stop.description)
    }
    var resumeReasons: [Reason] {
        return filterReasonsFor(key: TypeEnum.resume.description)
    }
    var revokeReasons: [Reason] {
        return filterReasonsFor(key: TypeEnum.revoke.description)
    }
    var deniedReasons: [Reason] {
        return filterReasonsFor(key: TypeEnum.denied.description)
    }
    
    // MARK: - Methods
    private func filterReasonsFor(key: String) -> [Reason] {
        var filteredReasons: [Reason] = []
        if let allReasons = self.filter({ $0.name == key }).first {
            let keys = allReasons.nomenclatures.map({ $0.name })
            for name in Set(keys) {
                if var reason = allReasons.nomenclatures.filter({ $0.name == name && $0.language == LanguageManager.preferredLanguage?.rawValue }).first {
                    reason.type = TypeEnum.getType(reasonTypeName: key)
                    filteredReasons.append(reason)
                }
            }
        }
        
        if let other = filteredReasons.filter({ $0.name.contains("OTHER") }).first {
            filteredReasons.move(other, to: filteredReasons.endIndex - 1)
        }
        
        return filteredReasons
    }
}

struct ReasonType: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var name: String
    var nomenclatures: [Reason]
}

struct Reason: Codable, Hashable {
    // MARK: - Properties
    var id: String
    var language: String
    var name: String
    var description: String
    var textRequired: Bool
    var permittedUser: ReasonPermittedUser
    var type: TypeEnum
    
    enum CodingKeys: String, CodingKey {
        case id
        case language
        case name
        case description
        case textRequired
        case permittedUser
    }
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        id =  try container.decode(String.self, forKey: .id)
        language =  try container.decode(String.self, forKey: .language)
        name =  try container.decode(String.self, forKey: .name)
        description =  try container.decode(String.self, forKey: .description)
        textRequired =  try container.decode(Bool.self, forKey: .textRequired)
        permittedUser =  try container.decode(ReasonPermittedUser.self, forKey: .permittedUser)
        type = .none
    }
}

enum TypeEnum {
    case stop
    case resume
    case revoke
    case denied
    case none
    
    var description: String {
        switch self {
        case .stop:
            "STOP_REASON_TYPE"
        case .resume:
            "RESUME_REASON_TYPE"
        case .revoke:
            "REVOKE_REASON_TYPE"
        case .denied:
            "DENIED_REASON_TYPE"
        case .none:
            ""
        }
    }
    
    static func getType(reasonTypeName: String) -> TypeEnum {
        switch reasonTypeName {
        case "STOP_REASON_TYPE":
            return .stop
        case "RESUME_REASON_TYPE":
            return .resume
        case "REVOKE_REASON_TYPE":
            return .revoke
        case "DENIED_REASON_TYPE":
            return .denied
        default:
            return .none
        }
    }
}


enum ReasonPermittedUser: String, Codable {
    case privateReason = "PRIVATE"
    case adminReason = "ADMIN"
    case publicReason = "PUBLIC"
}
