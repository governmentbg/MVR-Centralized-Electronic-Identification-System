//
//  NotificationTypesView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import SwiftUI


struct NotificationTypesView: View {
    // MARK: - Properties
    @StateObject private var viewModel = NotificationTypesViewModel()
    @SceneStorage("expandedNotificationTypeRows") var expandedRows: Set<String> = []
    
    // MARK: - Body
    var body: some View {
        ScrollView {
            LazyVStack(spacing: 0) {
                ForEach(viewModel.notificationTypesDisplayModels.indices, id: \.self) { index in
                    NotificationTypeRow(notificationType: $viewModel.notificationTypesDisplayModels[index],
                                        toggleEvent: { eventId in viewModel.toggleEvent(id: eventId) },
                                        toggleType: { viewModel.toggleType(id: viewModel.notificationTypesDisplayModels[index].id) })
                    .onAppear {
                        if viewModel.notificationTypesDisplayModels[index].id == viewModel.notificationTypesDisplayModels.last?.id {
                            viewModel.scrollViewBottomReached()
                        }
                    }
                }
            }
            .padding(.horizontal)
        }
        .refreshable {
            viewModel.getNotificationTypes()
        }
        .background(Color.backgroundWhite)
        .onAppear {
            viewModel.getNotificationTypes()
        }
        .onDisappear {
            expandedRows.removeAll()
        }
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
    }
}



// MARK: - Preview
#Preview {
    NotificationTypesView()
}


