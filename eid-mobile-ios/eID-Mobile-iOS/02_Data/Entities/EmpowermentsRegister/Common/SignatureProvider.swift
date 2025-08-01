//
//  SignatureProvider.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 19.12.23.
//

import Foundation


enum SignatureProvider: String, Codable {
    case kep = "KEP"
    case evrotrust = "Evrotrust"
    case borica = "Borica"
}

extension SignatureProvider {
    var title: String {
        switch self {
        case .kep:
            return ""
        case .evrotrust:
            return "signature_provider_eurotrust"
        case .borica:
            return "signature_provider_borica"
        }
    }
    
    var appName: String {
        switch self {   
        case .evrotrust:
            return "Evrotrust"
        case .borica:
            return "B-Trust"
        default:
            return ""
        }
    }
}
