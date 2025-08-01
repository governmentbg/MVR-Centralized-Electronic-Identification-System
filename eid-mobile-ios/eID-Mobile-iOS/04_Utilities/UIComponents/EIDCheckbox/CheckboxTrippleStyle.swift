//
//  CheckboxTrippleStyle.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import SwiftUI

/** States for tri-state checkbox: checked, unchecked and semichecked */
enum TrippleCheckboxState {
    case unchecked
    case semichecked
    case checked
    
    var imageName: String {
        switch self {
        case .unchecked:
            return "icon_checkbox_off"
        case .semichecked:
            return "icon_checkbox_semichecked"
        case .checked:
            return "icon_checkbox_on"
        }
    }
}


struct CheckboxTrippleStyle: ButtonStyle {
    // MARK: - Properties
    @Binding var state: TrippleCheckboxState
    var action: () -> Void
    @Environment(\.isEnabled) private var isEnabled: Bool
    
    // MARK: - Body
    func makeBody(configuration: Configuration) -> some View {
        Button(action: {
            action()
        }, label: {
            HStack {
                Image(isEnabled ? state.imageName : "icon_checkbox_disabled")
                configuration.label
            }
        })
        .padding()
        .background(isEnabled ? Color.clear : Color.backgroundLightGrey)
    }
}
