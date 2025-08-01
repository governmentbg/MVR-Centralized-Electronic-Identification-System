//
//  ButtonSize.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 12.09.23.
//

import SwiftUI


enum ButtonSize {
    case small
    case large
}

extension ButtonSize {
    var padding: EdgeInsets {
        switch self {
        case .small:
            return EdgeInsets(top: 8,
                              leading: 12,
                              bottom: 8,
                              trailing: 12)
        case .large:
            return EdgeInsets(top: 12,
                              leading: 12,
                              bottom: 12,
                              trailing: 12)
        }
    }
    
    var textFont: Font {
        switch self {
        case .small:
            return .bodySmall
        case .large:
            return .bodyRegular
        }
    }
}
