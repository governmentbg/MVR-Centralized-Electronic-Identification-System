//
//  NotificationSettingsView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 10.10.23.
//

import SwiftUI


struct NotificationSettingsView: View {
    // MARK: - Properties
    private var segments = NotificationSettingsSegments.allCases
    @State private var selectedTab = 0
    @Environment(\.presentationMode) var presentationMode
    @State var showInfo: Bool = false
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            EIDSegmentedPicker(segments,
                               selection: Binding(get: { selectedTab },
                                                  set: { selectedTab = $0 }),
                               content: { element, isSelected in
                HStack {
                    Image(element.iconName)
                        .tint(isSelected ? .textActive : .textLight)
                    Text(element.title)
                        .font(.heading4)
                        .foregroundStyle(isSelected ? Color.textActive : Color.textLight)
                }
                .frame(maxWidth: .infinity)
                .padding()
                .border(width: 2,
                        edges: [.bottom],
                        color: isSelected ? Color.textActive : Color.themePrimaryLight)
            })
            .padding(.horizontal)
            
            switch selectedTab {
            case 1:
                NotificationTypesView()
                    .frame(maxWidth: .infinity, maxHeight: .infinity)
            default:
                NotificationChannelsView()
                    .frame(maxWidth: .infinity, maxHeight: .infinity)
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.backgroundWhite)
        .addNavigationBar(title: "more_notification_settings_menu".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
            ToolbarItem(placement: .topBarTrailing) {
                Button(action: {
                    showInfo = true
                }, label: {
                    Image("icon_info")
                        .renderingMode(.template)
                        .foregroundColor(Color.textWhite)
                })
            }
        })
        .presentInfoView(showInfo: $showInfo,
                         title: .constant("info_title".localized()),
                         description: .constant("notification_info_description".localized()))
    }
}


// MARK: - Preview
#Preview {
    NotificationSettingsView()
}
