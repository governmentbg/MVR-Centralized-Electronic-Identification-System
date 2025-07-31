//
//  LogsBaseView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.10.24.
//

import SwiftUI


struct LogsBaseView<Content: View>: View {
    // MARK: - Properties
    @StateObject var viewModel: LogsBaseViewModel
    @Environment(\.presentationMode) var presentationMode
    @ViewBuilder let content: Content
    
    // MARK: - Body
    init(viewModel: LogsBaseViewModel,
         content: Content) {
        _viewModel = StateObject(wrappedValue: viewModel)
        self.content = content
    }
    
    var body: some View {
        ZStack(alignment: .bottomTrailing) {
            VStack(spacing: 0) {
                controlsView
                content
            }
            if viewModel.showFilter {
                filterView
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: viewModel.screenTitle,
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .onAppear {
            viewModel.getLogs(fromStart: true)
        }
    }
    
    // MARK: - Child views
    private var filterView: some View {
        return LogsFilterView(logsDescriptions: $viewModel.logsDescriptions,
                       startDateStr: $viewModel.startDateStr,
                       endDateStr: $viewModel.endDateStr,
                              type: $viewModel.type) {
            viewModel.showFilter = false
            viewModel.reloadDataFromStart()
        } dismiss: {
            viewModel.showFilter = false
        }
    }
    
    private var controlsView: some View {
        HStack {
            Spacer()
            EIDFilterButton(isFilterApplied: $viewModel.isFilterApplied,
                            action: { viewModel.showFilter = true })
        }
        .padding()
    }
}
