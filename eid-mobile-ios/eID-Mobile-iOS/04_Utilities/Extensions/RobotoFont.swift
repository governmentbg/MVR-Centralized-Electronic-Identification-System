//
//  RobotoFont.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.09.23.
//

import Foundation
import SwiftUI


// MARK: - RobotoFont
extension Font {
    enum RobotoFont {
        case regular
        case thin
        case light
        case medium
        case bold
        case black
        case custom(String)
        
        var value: String {
            switch self {
            case .regular:
                return "Roboto-Regular"
            case .thin:
                return "Roboto-Thin"
            case .light:
                return "Roboto-Light"
            case .medium:
                return "Roboto-Medium"
            case .bold:
                return "Roboto-Bold"
            case .black:
                return "Roboto-Black"
            case .custom(let name):
                return name
            }
        }
    }
    
    static func roboto(_ type: RobotoFont, size: CGFloat) -> Font {
        return .custom(type.value, size: size)
    }
}


// MARK: - RobotoCondensedFont
extension Font {
    enum RobotoCondensedFont {
        case regular
        case light
        case medium
        case bold
        case custom(String)
        
        var value: String {
            switch self {
            case .regular:
                return "RobotoCondensed-Regular"
            case .light:
                return "RobotoCondensed-Light"
            case .medium:
                return "RobotoCondensed-Medium"
            case .bold:
                return "RobotoCondensed-Bold"
            case .custom(let name):
                return name
            }
        }
    }
    
    static func robotoCondensed(_ type: RobotoCondensedFont, size: CGFloat) -> Font {
        return .custom(type.value, size: size)
    }
}

