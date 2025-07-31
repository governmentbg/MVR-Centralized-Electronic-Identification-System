//
//  ButtonConfig.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 12.09.23.
//

import Foundation
import SwiftUI


struct ButtonConfig {
    var buttonType: ButtonType
    var buttonState: ButtonState
}

extension ButtonConfig {
    var colors: ButtonColorConfig {
        switch buttonType {
        case .filled:
            return ButtonColorConfig(backgroundDefault: buttonState.colorDefault,
                                     backgroundPressed: buttonState.colorSelected,
                                     backgroundDisabled: .buttonDisabled,
                                     borderDefault: buttonState.colorDefault,
                                     borderPressed: buttonState.colorSelected,
                                     borderDisabled: .buttonDisabled,
                                     textDefault: buttonState.colorText,
                                     textPressed: buttonState.colorText,
                                     textDisabled: .textLight)
        case .outline:
            return ButtonColorConfig(backgroundDefault: .clear,
                                     backgroundPressed: .backgroundLightGrey,
                                     backgroundDisabled: .clear,
                                     borderDefault: buttonState.colorDefault,
                                     borderPressed: buttonState.colorSelected,
                                     borderDisabled: .buttonDisabled,
                                     textDefault: buttonState.colorDefault,
                                     textPressed: buttonState.colorSelected,
                                     textDisabled: .textLight)
        case .opaqueOutline:
            return ButtonColorConfig(backgroundDefault: .white,
                                     backgroundPressed: .backgroundLightGrey,
                                     backgroundDisabled: .white,
                                     borderDefault: buttonState.colorDefault,
                                     borderPressed: buttonState.colorSelected,
                                     borderDisabled: .buttonDisabled,
                                     textDefault: buttonState.colorDefault,
                                     textPressed: buttonState.colorSelected,
                                     textDisabled: .textLight)
        case .text:
            return ButtonColorConfig(backgroundDefault: .clear,
                                     backgroundPressed: .backgroundLightGrey,
                                     backgroundDisabled: .clear,
                                     borderDefault: .clear,
                                     borderPressed: .backgroundLightGrey,
                                     borderDisabled: .buttonDisabled,
                                     textDefault: buttonState.colorDefault,
                                     textPressed: buttonState.colorSelected,
                                     textDisabled: .textLight)
        }
    }
}


struct ButtonColorConfig {
    var backgroundDefault: Color
    var backgroundPressed: Color
    var backgroundDisabled: Color
    var borderDefault: Color
    var borderPressed: Color
    var borderDisabled: Color
    var textDefault: Color
    var textPressed: Color
    var textDisabled: Color
}
