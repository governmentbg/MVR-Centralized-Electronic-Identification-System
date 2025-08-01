//
//  NotificationTypeItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import SwiftUI


struct NotificationTypeItem: View {
    // MARK: - Properties
    @State var name: String
    @State var isDisabled: Bool = false
    @Binding var state: TrippleCheckboxState
    var action: () -> Void
    
    // MARK: - Body
    var body: some View {
        HStack {
            Button(action: {},
                   label: {})
            .buttonStyle(CheckboxTrippleStyle(state: $state,
                                              action: action))
            Text(name)
                .font(.bodySmall)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.leading)
            Spacer()
        }
        .frame(maxWidth: .infinity)
        .background(isDisabled ? Color.backgroundLightGrey : Color.backgroundWhite)
        .disabled(isDisabled)
    }
}


// MARK: - Preview
#Preview {
    NotificationTypeItem(name: "Type",
                         state: .constant(.checked),
                         action: {})
}
