//
//  ButtonStyle.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 12.09.23.
//

import SwiftUI


enum ButtonState {
    case primary
    case secondary
    case success
    case warning
    case danger
    case glowingWhite
}

extension ButtonState {
    var colorDefault: Color {
        switch self {
        case .primary:
            return .buttonDefault
        case .secondary:
            return .buttonDark
        case .success:
            return .buttonConfirm
        case .warning:
            return .buttonDanger
        case .danger:
            return .buttonReject
        case .glowingWhite:
            return .backgroundWhite
        }
    }
    
    var colorSelected: Color {
        switch self {
        case .primary:
            return .buttonDefaultHover
        case .secondary:
            return .buttonDarkHover
        case .success:
            return .buttonConfirmHover
        case .warning:
            return .buttonDangerHover
        case .danger:
            return .buttonRejectHover
        case .glowingWhite:
            return .backgroundWhite
        }
    }
    
    var colorText: Color {
        switch self {
        case .primary:
            return .textWhite
        case .secondary:
            return .textWhite
        case .success:
            return .textWhite
        case .warning:
            return .textDark
        case .danger:
            return .textWhite
        case .glowingWhite:
            return .textActive
        }
    }
}
