//
//  NotificationEventItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import SwiftUI


struct NotificationEventItem: View {
    // MARK: - Properties
    @Binding var event: NotificationEventDisplayModel
//    @State var name: String
//    @State var isOn: Bool
//    @State var isDisabled: Bool = false
    var onToggleChange: () -> Void
    
    // MARK: - Body
    var body: some View {
        HStack {
            Toggle(isOn: $event.isOn){
                Text(event.name)
                    .font(.bodySmall)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textDefault)
                    .multilineTextAlignment(.leading)
            }
            .toggleStyle(CheckboxToggleStyle(action: onToggleChange))
            Spacer()
        }
        .padding(.leading)
        .frame(maxWidth: .infinity)
        .border(width: 1,
                edges: [.bottom],
                color: .themePrimaryLight)
        .background(event.isMandatory ? Color.backgroundLightGrey : Color.backgroundWhite)
        .disabled(event.isMandatory)
    }
}


// MARK: - Preview
#Preview {
    NotificationEventItem(event: .constant(NotificationEventDisplayModel(id: "id", name: "Name")),
                          onToggleChange: {})
}
