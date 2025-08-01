//
//  CertificateAction.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 23.04.24.
//

import SwiftUI


enum CertificateAction {
    case stop
    case resume
    case revoke
    case changePIN
    case noAction
}

extension CertificateAction {
    static func action(status: CertificateStatus) -> CertificateAction {
        switch status {
        case .active:
            return .stop
        case .stopped:
            return .resume
        default:
            return .noAction
        }
    }
    
    var icon: String {
        switch self {
        case .stop:
            return "icon_paused"
        case .resume:
            return "icon_resume"
        case .revoke:
            return "icon_stop"
        case .changePIN:
            return "icon_edit"
        case .noAction: return ""
        }
    }
    
    var textColor: Color {
        switch self {
        case .stop:
            return .buttonDanger
        case .resume:
            return .buttonConfirm
        case .revoke:
            return .buttonReject
        case .changePIN:
            return .buttonDefault
        case .noAction: return .clear
        }
    }
    
    var buttonTitle: String {
        switch self {
        case .stop:
            return "certificate_action_stop"
        case .resume:
            return "certificate_action_resume"
        case .revoke:
            return "certificate_action_revoke"
        case .changePIN:
            return "change_card_pin_title"
        case .noAction: return ""
        }
    }
    
    var certificateDetailsViewState: CertificateDetailsViewModel.ViewState {
        switch self {
        case .stop:
            return .stopCertificate
        case .resume:
            return .resumeCertificate
        case .revoke:
            return .revokeCertificate
        default:
            return .preview
        }
    }
}
