//
//  PendingView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 30.07.24.
//

import SwiftUI


struct PendingView: View {
    // MARK: - Properties
    @StateObject var viewModel = PendingViewModel()
    
    // MARK: - Body
    var body: some View {
        NavigationStack {
            ZStack(alignment: .bottomTrailing) {
                VStack(spacing: 0) {
                    ScrollView {
                        if viewModel.requests.count == 0 && !viewModel.showLoading {
                            EmptyListView()
                        }
                        VStack(spacing: 0) {
                            ForEach(viewModel.requests, id: \.self) { request in
                                PendingApprovalRequestItem(title:
                                                            viewModel.getRequestTitle(request: request),
                                                           details: request.requestFrom?.system?.localisedDescription ?? "",
                                                           creationDate: request.createDate ?? "",
                                                           action: { state in
                                    viewModel.selectedRequest = request
                                    switch state {
                                    case .cancelled:
                                        viewModel.setRequestState(state: .cancelled)
                                    case .succeed:
                                        switch request.levelOfAssurance {
                                        case .low, .substantial:
                                            viewModel.showSignMethodPicker = true
                                        case .high:
                                            viewModel.showPINView = true
                                        }
                                    }
                                })
                            }
                        }
                    }
                    .refreshable {
                        viewModel.getRequests()
                        viewModel.startTimer()
                    }
                }
                .padding([.top], 8)
            }
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .background(Color.themeSecondaryLight)
            .addNavigationBar(title: "pending_approval_requests_screen_title".localized(),
                              content: {
                ToolbarItem(placement: .topBarTrailing) {
                    EmptyView()
                }
            })
            .onAppear {
                viewModel.getRequests()
                viewModel.startTimer()
            }
            .onDisappear {
                viewModel.stopTimer()
            }
            .observeLoading(isLoading: $viewModel.showLoading)
            .presentAlert(showAlert: $viewModel.showError,
                          alertText: $viewModel.errorText)
            .presentAlert(showAlert: $viewModel.showSuccess,
                          alertText: $viewModel.successText, onDismiss: {
                viewModel.getRequests()
            })
            .presentActionSheet(showOptions: $viewModel.showSignMethodPicker,
                                title: "request_sign_method_picker_title".localized(),
                                options: PendingRequestSignMethod.allCases.map({ $0.title }),
                                onOptionPicked: { option in
                if let method = PendingRequestSignMethod.allCases.first(where: { $0.title == option }) {
                    viewModel.selectedSignMethod = method
                    viewModel.showPINView = true
                }
            })
            .sheet(isPresented: $viewModel.showPINView, content: {
                EIDPINView(viewModel: EIDPINViewModel(state: .approveAuthRequest(viewModel.selectedSignMethod), onAuthRequest: { request in
                    DispatchQueue.main.asyncAfter(deadline: .now() + 2.5) {
                        viewModel.showPINView = false
                        viewModel.setRequestState(state: .succeed, request: request)
                    }
                }))
            })
        }
    }
}
