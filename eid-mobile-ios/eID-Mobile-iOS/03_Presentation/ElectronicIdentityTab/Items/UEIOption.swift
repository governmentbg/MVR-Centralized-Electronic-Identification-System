//
//  EIDManagementOption.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.02.24.
//

import Foundation


enum EIDManagementOption {
    case applications
    case certificates
    
    var title: String {
        switch self {
        case .applications:
            return "uei_applications_title"
        case .certificates:
            return "uei_certificates_title"
        }
    }
    
    var subtitle: String {
        switch self {
        case .applications:
            return "uei_applications_subtitle"
        case .certificates:
            return "uei_certificates_subtitle"
        }
    }
    
    var iconName: String {
        switch self {
        case .applications:
            return "icon_uei_applications"
        case .certificates:
            return "icon_uei_certificates"
        }
    }
}
