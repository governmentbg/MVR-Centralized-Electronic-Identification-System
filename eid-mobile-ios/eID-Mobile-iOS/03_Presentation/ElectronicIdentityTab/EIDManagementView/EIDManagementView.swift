//
//  EIDManagementView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.02.24.
//

import SwiftUI


enum EIDManagementDestinations: String {
    case applications = "Applications"
    case certificates = "Certificates"
    case createApplication = "Create application"
}


struct EIDManagementView: View {
    // MARK: - Properties
    @StateObject var viewModel = EIDManagementViewModel()
    @Environment(\.presentationMode) var presentationMode
    @State private var navPath: [String] = []
    
    // MARK: - Body
    var body: some View {
        NavigationStack(path: $navPath) {
            VStack {
                NavigationLink(value: EIDManagementDestinations.applications.rawValue) {
                    ListMenuItem(imageName: EIDManagementOption.applications.iconName,
                                 title: EIDManagementOption.applications.title.localized(),
                                 subtitle: EIDManagementOption.applications.subtitle.localized())
                }
                .buttonStyle(ListMenuButtonStyle())
                
                NavigationLink(value: EIDManagementDestinations.certificates.rawValue) {
                    ListMenuItem(imageName: EIDManagementOption.certificates.iconName,
                                 title: EIDManagementOption.certificates.title.localized(),
                                 subtitle: EIDManagementOption.certificates.subtitle.localized())
                }
                .buttonStyle(ListMenuButtonStyle())
                
                Spacer()
                
                VStack {
                    //                    NavigationLink(destination: CompleteApplicationView(),
                    //                                   label: {
                    //                        Text("btn_scan_qr_code".localized())
                    //                    })
                    //                    .buttonStyle(EIDButton(buttonType: .outline))
                    NavigationLink(value: EIDManagementDestinations.createApplication.rawValue) {
                        Text("btn_new_certificate".localized())
                    }
                    .buttonStyle(EIDButton(buttonType: .filled,
                                           buttonState: .success))
                }
            }
            .padding()
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .background(Color.themeSecondaryLight)
            .addNavigationBar(title: "uei_screen_title".localized(),
                              content: {
                ToolbarItem(placement: .topBarTrailing) {
                    Button(action: {
                        viewModel.showInfo = true
                    }, label: {
                        Image("icon_info")
                            .renderingMode(.template)
                            .foregroundColor(Color.white)
                    })
                }
            })
            .onAppear {
                viewModel.getHelperData()
            }
            .navigationDestination(for: String.self) { pathValue in
                // depending on the value you pass you will navigate accrodingly
                switch pathValue {
                case EIDManagementDestinations.applications.rawValue:
                    ApplicationsView(viewModel: ApplicationsViewModel(administrators: viewModel.administrators,
                                                                      devices: viewModel.devices,
                                                                      reasons: viewModel.reasons),
                                     path: $navPath)
                case EIDManagementDestinations.certificates.rawValue:
                    CertificatesView(viewModel: CertificatesViewModel(administrators: viewModel.administrators,
                                                                      devices: viewModel.devices,
                                                                      reasons: viewModel.reasons),
                                     path: $navPath)
                case EIDManagementDestinations.createApplication.rawValue:
                    CreateApplicationView(viewModel: CreateApplicationViewModel(administrators: viewModel.administrators,
                                                                                devices: viewModel.devices),
                                          path: $navPath)
                default:
                    EmptyView()
                }
            }
            .presentInfoView(showInfo: $viewModel.showInfo,
                             title: .constant("info_title".localized()),
                             description: .constant("eid_management_info_description".localized()))
        }
    }
}


// MARK: - Preview
#Preview {
    EIDManagementView()
}
