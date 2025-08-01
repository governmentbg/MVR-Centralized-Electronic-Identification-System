//
//  NotificationChannelsView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import SwiftUI


struct NotificationChannelsView: View {
    // MARK: - Properties
    @StateObject private var viewModel = NotificationChannelsViewModel()
    
    // MARK: - Body
    var body: some View {
        ScrollView {
            VStack(spacing: 0) {
                ForEach($viewModel.channelsDisplayModels, id: \.self) { $channel in
                    if #available(iOS 17, *) {
                        NotificationChannelItem(name: channel.name,
                                                description: channel.description,
                                                isOn: $channel.isSelected,
                                                isDisabled: channel.isMandatory,
                                                onToggleChange: { viewModel.toggleChannel(id: channel.id, isOn: channel.isSelected) })
                    } else {
                        DeprecatedNotificationChannelItem(name: channel.name,
                                                          description: channel.description,
                                                          isOn: $channel.isSelected,
                                                          isDisabled: channel.isMandatory,
                                                          onToggleChange: { viewModel.toggleChannel(id: channel.id, isOn: channel.isSelected) })
                    }
                }
            }
            .padding(.horizontal)
        }
        .background(Color.backgroundWhite)
        .onAppear {
            viewModel.getNotificationChannels()
        }
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
    }
}


// MARK: - Preview
#Preview {
    NotificationChannelsView()
}
