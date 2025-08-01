//
//  CertificatesView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.02.24.
//

import SwiftUI


struct CertificatesView: View {
    // MARK: - Properties
    @StateObject var viewModel: CertificatesViewModel
    @Environment(\.presentationMode) var presentationMode
    @Binding var path: [String]
    @State private var targetCertificate: CertificateResponse?
    @State private var selectedAction: CertificateAction = .noAction
    @State private var hideCertificateItemOptions: Bool = false
    
    // MARK: - Body
    var body: some View {
        ZStack(alignment: .bottomTrailing) {
            VStack(spacing: 0) {
                controlsView
                ScrollView {
                    if viewModel.certificates.count == 0 && !viewModel.showLoading {
                        EmptyListView()
                    }
                    LazyVStack {
                        ForEach(viewModel.certificates, id: \.self) { certificate in
                            certificateRow(certificate: certificate)
                        }
                    }
                }
                .refreshable {
                    viewModel.getCertificates()
                }
            }
            .navigationDestination(isPresented: $viewModel.handleAction) {
                switch selectedAction {
                case .changePIN:
                    ChangeCertificatePINView(device: viewModel.selectedDeviceForPinChange)
                default:
                    if let details = viewModel.targetCertificateDetails {
                        CertificateDetailsView(
                            viewModel: CertificateDetailsViewModel(
                                certificateDetails: details,
                                certificateHistory: viewModel.targetCertificateHistory ?? [],
                                state: selectedAction.certificateDetailsViewState,
                                administrators: viewModel.administrators,
                                devices: viewModel.devices,
                                reasons: viewModel.reasons),
                            path: $path)
                    }
                }
            }
            if viewModel.showFilter {
                filterView
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "uei_certificates_screen_title".localized(),
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
            hideCertificateItemOptions = false
            selectedAction = .noAction
            viewModel.getCertificates()
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
            ForEach(CertificateSortCriteria.allCases, id: \.self) { criteria in
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
        CertificatesFilterView(administrators: $viewModel.administrators,
                               devices: $viewModel.devices,
                               status: $viewModel.status,
                               serialNumber: $viewModel.serialNumber,
                               validFromDateStr: $viewModel.validityFrom,
                               validUntilDateStr: $viewModel.validityUntil,
                               deviceId: $viewModel.deviceId,
                               certificateName: $viewModel.certificateName,
                               selectedAdministrator: $viewModel.selectedAdministrator,
                               applyFilter: {
            viewModel.showFilter = false
            viewModel.reloadDataFromStart()
        },
                               dismiss: {
            viewModel.showFilter = false
        })
    }
    
    private func certificateRow(certificate: CertificateResponse) -> some View {
        Button(action: {
            viewModel.getCertificateDetails(forId: certificate.id)
        }, label: {
            CertificateItem(serialNumber: certificate.serialNumber,
                            validFrom: certificate.validityFrom,
                            validUntil: certificate.validityUntil,
                            deviceName: viewModel.getDeviceBy(id: certificate.deviceId).name,
                            alias: certificate.alias ?? "",
                            status: certificate.status,
                            isExpiring: certificate.isExpiring ?? false,
                            validCertificateOwner: viewModel.checkValidCertificateOwner(id: certificate.deviceId),
                            hideItemMenuOptions: $hideCertificateItemOptions) { action in
                selectedAction = action
                hideCertificateItemOptions = true
                
                if action == .changePIN {
                    viewModel.changeDevicePin(id: certificate.deviceId)
                } else {
                    viewModel.getCertificateDetails(forId: certificate.id,
                                                    includeHistory: false)
                }
            }
        })
        .addItemShadow()
        .onAppear {
            if certificate.id == viewModel.certificates.last?.id {
                viewModel.scrollViewBottomReached()
            }
        }
    }
}


// MARK: - Preview
#Preview {
    CertificatesView(viewModel: CertificatesViewModel(administrators: [],
                                                      devices: [],
                                                      reasons: []),
                     path: .constant([""]))
}
