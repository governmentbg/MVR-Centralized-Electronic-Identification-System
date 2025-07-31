//
//  ApplicationsView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.02.24.
//

import SwiftUI


struct ApplicationsView: View {
    // MARK: - Properties
    @StateObject var viewModel: ApplicationsViewModel
    @Environment(\.presentationMode) var presentationMode
    @State private var hideApplicationItemOptions: Bool = false
    @Binding var path: [String]
    
    // MARK: - Body
    var body: some View {
        ZStack(alignment: .bottomTrailing) {
            VStack(spacing: 0) {
                controlsView
                ScrollView {
                    if viewModel.applications.count == 0 && !viewModel.showLoading {
                        EmptyListView()
                    }
                    LazyVStack {
                        ForEach(viewModel.applications, id: \.self) { application in
                            applicationRow(application: application)
                        }
                    }
                }
                .refreshable {
                    viewModel.getApplications()
                }
            }
            if viewModel.showFilter {
                filterView
            }
        }
        .navigationDestination(isPresented: $viewModel.handleAction) {
            if let details = viewModel.targetApplicationDetails {
                ApplicationDetailsView(
                    viewModel:
                        ApplicationDetailsViewModel(
                            applicationDetails: details,
                            administrators: viewModel.administrators,
                            devices: viewModel.devices,
                            reasons: viewModel.reasons),
                    path: $path)
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "uei_applications_screen_title".localized(),
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
            viewModel.getApplications()
        }
    }
    
    // MARK: - Child views
    private var controlsView: some View {
        HStack {
            sortView
            Spacer()
            EIDFilterButton(isFilterApplied: $viewModel.isFilterApplied,
                            action: { viewModel.showFilter = true })
        }
        .padding()
    }
    
    private var sortView: some View {
        Menu(content: {
            Button(action: {
                hideKeyboard()
                viewModel.sortCriteria = nil
                viewModel.sortDirection = nil
                viewModel.reloadDataFromStart()
            }, label: {
                HStack {
                    Text("sort_by_default_title".localized())
                    if viewModel.sortCriteria == nil && viewModel.sortDirection == nil {
                        Image("icon_selected_blue")
                    }
                }
            })
            ForEach(ApplicationSortCriteria.allCases, id: \.self) { criteria in
                Button(action: {
                    viewModel.sortCriteria = criteria
                    viewModel.sortDirection = .asc
                    viewModel.reloadDataFromStart()
                }, label: {
                    HStack {
                        Text("\(criteria.title.localized())  (\(SortDirection.asc.title.localized()))")
                        if viewModel.sortCriteria == criteria && viewModel.sortDirection == .asc {
                            Image("icon_selected_blue")
                        }
                    }
                })
                Button(action: {
                    viewModel.sortCriteria = criteria
                    viewModel.sortDirection = .desc
                    viewModel.reloadDataFromStart()
                }, label: {
                    HStack {
                        Text("\(criteria.title.localized())  (\(SortDirection.desc.title.localized()))")
                        if viewModel.sortCriteria == criteria && viewModel.sortDirection == .desc {
                            Image("icon_selected_blue")
                        }
                    }
                })
            }
        }, label: {
            HStack(spacing: 0) {
                Text(String(format: "sort_by_title".localized(), viewModel.sortTitle))
                    .font(.tiny)
                    .lineSpacing(4)
                    .multilineTextAlignment(.leading)
                    .foregroundStyle(Color.textLight)
                Image("icon_arrow_down_small")
            }
        })
        .preferredColorScheme(.light)
    }
    
    private var filterView: some View {
        ApplicationsFilterView(administrators: $viewModel.administrators,
                               devices: $viewModel.devices,
                               status: $viewModel.status,
                               id: $viewModel.id,
                               applicationNumber: $viewModel.applicationNumber,
                               createDateText: $viewModel.createDateText,
                               deviceId: $viewModel.deviceId,
                               applicationType: $viewModel.applicationType,
                               selectedAdministrator: $viewModel.selectedAdministrator,
                               applyFilter: {
            viewModel.showFilter = false
            viewModel.reloadDataFromStart()
        },
                               dismiss: {
            viewModel.showFilter = false
        })
    }
    
    private func applicationRow(application: ApplicationResponse) -> some View {
        Button(action: {
            viewModel.getApplicationDetails(forId: application.id)
        }, label: {
            ApplicationItem(
                applicationNumber: application.applicationNumber,
                applicationType: application.applicationType,
                creationDate: application.createDate,
                administratorName: application.eidAdministratorName,
                deviceName: viewModel.devices.first(where: { $0.id == application.deviceId })?.name ?? "",
                status: application.status,
                hideItemMenuOptions: $hideApplicationItemOptions) { _ in
                    hideApplicationItemOptions = true
                    PaymentHelper.openPaymentView(paymentAccessCode: application.paymentAccessCode ?? "")
                }
        })
        .addItemShadow()
        .onAppear {
            if application.id == viewModel.applications.last?.id {
                viewModel.scrollViewBottomReached()
            }
        }
    }
}


// MARK: - Preview
#Preview {
    ApplicationsView(viewModel: ApplicationsViewModel(administrators: [],
                                                      devices: [],
                                                      reasons: []),
                     path: .constant([""]))
}
