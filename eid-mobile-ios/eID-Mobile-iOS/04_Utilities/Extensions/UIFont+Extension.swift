//
//  UIFont+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.10.23.
//

import Foundation
import SwiftUI


extension UIFont {
    static let hero = UIFont(name: Font.RobotoCondensedFont.medium.value, size: 36) ?? UIFont.systemFont(ofSize: 36, weight: .medium)
    
    static let heading1 = UIFont(name: Font.RobotoCondensedFont.medium.value, size: 32) ?? UIFont.systemFont(ofSize: 32, weight: .medium)
    static let heading2 = UIFont(name: Font.RobotoCondensedFont.medium.value, size: 24) ?? UIFont.systemFont(ofSize: 24, weight: .medium)
    static let heading3 = UIFont(name: Font.RobotoCondensedFont.medium.value, size: 20) ?? UIFont.systemFont(ofSize: 20, weight: .medium)
    static let heading4 = UIFont(name: Font.RobotoCondensedFont.medium.value, size: 16) ?? UIFont.systemFont(ofSize: 16, weight: .medium)
    static let heading5 = UIFont(name: Font.RobotoCondensedFont.regular.value, size: 16) ?? UIFont.systemFont(ofSize: 16, weight: .regular)
    static let heading6 = UIFont(name: Font.RobotoCondensedFont.regular.value, size: 14) ?? UIFont.systemFont(ofSize: 14, weight: .regular)
    
    static let bodyLarge = UIFont(name: Font.RobotoFont.bold.value, size: 16) ?? UIFont.systemFont(ofSize: 16, weight: .bold)
    static let bodyRegular = UIFont(name: Font.RobotoFont.regular.value, size: 16) ?? UIFont.systemFont(ofSize: 16, weight: .regular)
    static let bodySmall = UIFont(name: Font.RobotoFont.regular.value, size: 14) ?? UIFont.systemFont(ofSize: 14, weight: .regular)
    
    static let tiny = UIFont(name: Font.RobotoFont.regular.value, size: 12) ?? UIFont.systemFont(ofSize: 12, weight: .regular)
    static let label = UIFont(name: Font.RobotoCondensedFont.medium.value, size: 14) ?? UIFont.systemFont(ofSize: 14, weight: .medium)
}

