//
//  LogsToMeView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import SwiftUI


struct LogsToMeView: View {
    // MARK: - Properties
    @StateObject var viewModel: LogsToMeViewModel
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        LogsBaseView(viewModel: viewModel,
                     content: listView)
    }
    
    // MARK: - Child views
    private var listView: some View {
        ScrollView {
            if viewModel.logs.count == 0 && !viewModel.showLoading {
                EmptyListView()
            }
            LazyVStack {
                ForEach(viewModel.logs, id: \.self) { log in
                    LogToMeItem(eventId: log.eventId,
                                eventDate: log.eventDate,
                                eventSystemName: log.requesterSystemName,
                                eventType: viewModel.getEventTitle(log: log))
                    .addItemShadow()
                    .onAppear {
                        if log.id == viewModel.logs.last?.id {
                            viewModel.scrollViewBottomReached()
                        }
                    }
                }
            }
        }
        .refreshable {
            viewModel.getLogs(fromStart: true)
        }
    }
}
