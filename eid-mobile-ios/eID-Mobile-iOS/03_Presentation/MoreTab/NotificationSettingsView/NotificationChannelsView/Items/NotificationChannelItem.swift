//
//  NotificationChannelItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import SwiftUI


@available(iOS 17.0, *)
struct NotificationChannelItem: View {
    // MARK: - Properties
    @State var name: String
    @State var description: String
    @Binding var isOn: Bool
    @State var isDisabled: Bool = false
    var onToggleChange: () -> Void
    
    // MARK: - Body
    var body: some View {
        VStack {
            Toggle(isOn: $isOn.didSet(execute: { _ in
                onToggleChange()
            })) {
                VStack(alignment: .leading, spacing: 8) {
                    Text(name)
                        .font(.bodyLarge)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textDefault)
                        .multilineTextAlignment(.leading)
                    Text(description)
                        .font(.bodySmall)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textLight)
                        .multilineTextAlignment(.leading)
                }
            }
            .tint(Color.buttonDefault)
            .disabled(isDisabled)
        }
        .padding()
        .border(width: 1,
                edges: [.bottom],
                color: Color.themePrimaryLight)
        .background(isDisabled ? Color.backgroundLightGrey : Color.backgroundWhite)
    }
}


struct DeprecatedNotificationChannelItem: View {
    // MARK: - Properties
    @State var name: String
    @State var description: String
    @Binding var isOn: Bool
    @State var isDisabled: Bool = false
    var onToggleChange: () -> Void
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 8) {
            Toggle(isOn: $isOn.didSet(execute: { _ in
                onToggleChange()
            })) {
                VStack(alignment: .leading, spacing: 8) {
                    Text(name)
                        .font(.bodyLarge)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textDefault)
                    Text(description)
                        .font(.bodySmall)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textLight)
                }
            }
            .tint(Color.buttonDefault)
            .disabled(isDisabled)
        }
        .padding()
        .border(width: 1,
                edges: [.bottom],
                color: Color.themePrimaryLight)
        .padding([.leading, .trailing], 24)
        .background(isDisabled ? Color.backgroundLightGrey : Color.backgroundWhite)
    }
}


// MARK: - Preview
#Preview {
    if #available(iOS 17.0, *) {
        NotificationChannelItem(name: "Name",
                                description: "Description",
                                isOn: .constant(true),
                                onToggleChange: {})
    } else {
        DeprecatedNotificationChannelItem(name: "Name",
                                          description: "Description",
                                          isOn: .constant(true),
                                          onToggleChange: {})
    }
}
