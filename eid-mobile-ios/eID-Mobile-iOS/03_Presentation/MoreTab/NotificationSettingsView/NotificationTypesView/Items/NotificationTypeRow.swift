//
//  NotificationTypeRow.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 9.02.24.
//

import SwiftUI


struct NotificationTypeRow: View {
    // MARK: - Properties
    @Binding var notificationType: NotificationTypeDisplayModel
    var toggleEvent: ((String) -> Void)
    var toggleType: (() -> Void)
    // Persist expanded state
    @SceneStorage("expandedNotificationTypeRows") var expandedRows: Set<String> = []
    var isExpandedBinding: Binding<Bool> {
        Binding<Bool> {
            self.expandedRows.contains(self.notificationType.id)
        } set: { isExpanded in
            if isExpanded {
                self.expandedRows.insert(self.notificationType.id)
            } else {
                self.expandedRows.remove(self.notificationType.id)
            }
        }
    }
    
    // MARK: - Body
    var body: some View {
        DisclosureGroup(isExpanded: isExpandedBinding,
                        content: {
            VStack(spacing: 0) {
                ForEach(notificationType.events.indices, id: \.self) { index in
                    NotificationEventItem(event: $notificationType.events[index],
                                          onToggleChange: { toggleEvent(notificationType.events[index].id) })
                }
            }
        }, label: {
            NotificationTypeItem(name: notificationType.name,
                                 isDisabled: notificationType.isMandatory,
                                 state: $notificationType.state,
                                 action: { toggleType() })
        })
        .disclosureGroupStyle(EIDDisclosureGroupStyle())
        .background(notificationType.isMandatory ? Color.backgroundLightGrey : Color.backgroundWhite)
        .accentColor(Color.textDefault)
    }
}
