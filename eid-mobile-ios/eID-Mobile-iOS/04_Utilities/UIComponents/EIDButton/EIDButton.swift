//
//  EIDButton.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 12.09.23.
//

import SwiftUI


struct EIDButton: ButtonStyle {
    // MARK: - Properties
    var buttonType: ButtonType = .filled
    var size: ButtonSize = .large
    var buttonState: ButtonState = .primary
    var wide: Bool = true
    @Environment(\.isEnabled) private var isEnabled: Bool
    var btnConfig: ButtonConfig {
        return ButtonConfig(buttonType: buttonType,
                            buttonState: buttonState)
    }
    
    // MARK: - Init
    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .if(wide) { view in
                view.frame(maxWidth: .infinity)
            }
            .font(size.textFont)
            .padding(size.padding)
            .foregroundStyle(getForegroundColor(for: configuration))
            .background(getBackgroundColor(for: configuration))
            .border(getBorderColor(for: configuration),
                    width: buttonType.borderWidth)
            .clipShape(RoundedRectangle(cornerRadius: buttonType.cornerRadius))
            .if(buttonState == .glowingWhite) { view in
                view.addGlow()
            }
    }
    
    // MARK: - Helpers
    private func getForegroundColor(for configuration: Configuration) -> Color {
        guard isEnabled else {
            return btnConfig.colors.textDisabled
        }
        return configuration.isPressed ? btnConfig.colors.textPressed : btnConfig.colors.textDefault
    }
    
    private func getBackgroundColor(for configuration: Configuration) -> Color {
        guard isEnabled else {
            return btnConfig.colors.backgroundDisabled
        }
        return configuration.isPressed ? btnConfig.colors.backgroundPressed : btnConfig.colors.backgroundDefault
    }
    
    private func getBorderColor(for configuration: Configuration) -> Color {
        guard isEnabled else {
            return btnConfig.colors.borderDisabled
        }
        return configuration.isPressed ? btnConfig.colors.borderPressed : btnConfig.colors.borderDefault
    }
}
