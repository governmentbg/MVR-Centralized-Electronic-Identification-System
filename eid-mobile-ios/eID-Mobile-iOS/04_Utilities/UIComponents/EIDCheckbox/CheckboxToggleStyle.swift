//
//  CheckboxToggleStyle.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import SwiftUI


struct CheckboxToggleStyle: ToggleStyle {
    // MARK: - Properties
    var action: () -> Void
    @Environment(\.isEnabled) private var isEnabled: Bool
    
    // MARK: - Body
    func makeBody(configuration: Self.Configuration) -> some View {
        HStack {
            Image(getImageName(isOn: configuration.isOn))
            configuration.label
        }
        .padding([.top, .bottom], 8)
        .background(isEnabled ? Color.clear : Color.backgroundLightGrey)
        .onTapGesture {
            configuration.isOn.toggle()
            action()
        }
    }
    
    // MARK: - Helpers
    private func getImageName(isOn: Bool) -> String {
        guard isEnabled else {
            return "icon_checkbox_disabled"
        }
        return isOn ? "icon_checkbox_on" : "icon_checkbox_off"
    }
}
