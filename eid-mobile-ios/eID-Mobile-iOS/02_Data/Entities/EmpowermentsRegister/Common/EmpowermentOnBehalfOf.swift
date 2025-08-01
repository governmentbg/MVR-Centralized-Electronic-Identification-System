//
//  EmpowermentOnBehalfOf.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.11.23.
//

import Foundation


enum EmpowermentOnBehalfOf: String, Codable {
    case empty = "Empty"
    case individual = "Individual"
    case legalEntity = "LegalEntity"
}

extension EmpowermentOnBehalfOf {
    var title: String {
        switch self {
        case .individual:
            return "empowerment_authorizer_type_individual"
        case .legalEntity:
            return "empowerment_authorizer_type_legal_entity"
        case .empty:
            return "option_all"
        }
    }
    
    var info: String {
        switch self {
        case .individual:
            return "empowerment_info_individual_description"
        case .legalEntity:
            return "empowerment_info_legal_entity_description"
        default:
            return "empowerment_info_description"
        }
    }
}
