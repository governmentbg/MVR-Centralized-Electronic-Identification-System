//
//  EIDApplicationType.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.03.24.
//

import Foundation


enum EIDApplicationType: String, Codable, CaseIterable {
    case issue = "ISSUE_EID"
    case resume = "RESUME_EID"
    case revoke = "REVOKE_EID"
    case stop = "STOP_EID"
}

extension EIDApplicationType {
    var title: String {
        switch self {
        case .issue:
            return "application_type_issue_eid"
        case .resume:
            return "application_type_resume_eid"
        case .revoke:
            return "application_type_revoke_eid"
        case .stop:
            return "application_type_stop_eid"
        }
    }
    
    var screenTitle: String {
        switch self {
        case .issue:
            return "application_type_issue_eid_screen_title"
        case .resume:
            return "application_type_resume_eid_screen_title"
        case .revoke:
            return "application_type_revoke_eid_screen_title"
        case .stop:
            return "application_type_stop_eid_screen_title"
        }
    }
    
    var buttonTitle: String {
        switch self {
        case .issue:
            return "btn_continue_application"
        case .resume:
            return "btn_continue_application_resume"
        case .revoke:
            return "btn_continue_application_revoke"
        case .stop:
            return "btn_continue_application_stop"
        }
    }
    
    var successMessage: String {
        switch self {
        case .stop:
            return "certificate_stopped_success_title"
        case .resume:
            return "certificate_resumed_success_title"
        case .revoke:
            return "certificate_revoked_success_title"
        default:
            return ""
        }
    }
}
