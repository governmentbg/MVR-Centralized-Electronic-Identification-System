//
//  InputFieldState.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 18.09.23.
//

import SwiftUI


enum InputFieldState {
    case `default`
    case active
    case filled
    case disabled
    case error
}


extension InputFieldState {
    var titleColor: Color {
        return .textLight
    }
    
    var textColor: Color {
        switch self {
        case .default:
            return .textLight
        case .active:
            return .textActive
        case .filled:
            return .textDefault
        case .disabled:
            return .textDefault
        case .error:
            return .textActive
        }
    }
    
    var borderColor: Color {
        switch self {
        case .default:
            return .themePrimaryLight
        case .active:
            return .textActive
        case .filled:
            return .textActive
        case .disabled:
            return .themePrimaryLight
        case .error:
            return .textError
        }
    }
    
    var backgroundColor: Color {
        return self == .disabled ? .themePrimaryLight : .backgroundWhite
    }
}
