//
//  ButtonType.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 12.09.23.
//

import SwiftUI


enum ButtonType {
    case filled
    case outline
    case opaqueOutline
    case text
}

extension ButtonType {
    var cornerRadius: CGFloat {
        return self == .text ? 0 : 4
    }
    
    var borderWidth: CGFloat {
        return self == .text ? 0 : 1
    }
}
