//
//  MoreView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 26.10.23.
//

import SwiftUI


struct MoreView: View {
    // MARK: - Properteis
    private var menuGroups: [MoreMenuGroup] = MoreMenuGroup.allCases
    @State private var navPath: [String] = []
    
    // MARK: - Body
    var body: some View {
        NavigationStack(path: $navPath) {
            ScrollView {
                VStack(alignment: .leading, spacing: 24) {
                    ForEach(menuGroups, id: \.menuOptions) { menuGroup in
                        if menuGroup.title.isEmpty {
                            Divider()
                                .frame(height: 1)
                                .frame(maxWidth: .infinity)
                                .overlay(Color.themePrimaryMedium)
                        } else {
                            HStack {
                                if let imageName = menuGroup.imageName {
                                    Image(imageName)
                                        .renderingMode(.template)
                                        .resizable()
                                        .foregroundColor(Color.textInactive)
                                        .frame(width: 16, height: 16)
                                } else {
                                    Color.clear.frame(width: 16)
                                }
                                Text(menuGroup.title.localized().uppercased())
                                    .font(.tiny)
                                    .lineSpacing(4)
                                    .foregroundStyle(Color.textInactive)
                            }
                        }
                        VStack(alignment: .leading, spacing: 24) {
                            ForEach(menuGroup.menuOptions, id: \.self) { menuOption in
                                switch menuOption {
                                case .onlineHelpSystem:
                                    onlineHelpMenuButton(menuOption: menuOption)
                                default:
                                    NavigationLink(value: menuOption.destination.rawValue) {
                                        menuButtonView(menuOption: menuOption)
                                    }
                                }
                            }
                        }
                        .padding(.horizontal)
                    }
                }
                .padding()
            }
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .background(Color.themePrimaryDark)
            .addNavigationBar(title: "tab_title_more".localized(),
                              content: {
                ToolbarItem(placement: .topBarTrailing) {
                    EmptyView()
                }
            })
            .navigationDestination(for: String.self) { pathValue in
                if let destination = MoreMenuDestinations(rawValue: pathValue) {
                    switch destination {
                    case .profile:
                        ProfileView()
                    case .security:
                        DeviceSecurityView()
                    case .notificationSettings:
                        NotificationSettingsView()
                    case .empowermentsRegister:
                        EmpowermentsRegisterView(path: $navPath)
                    case .empowermentsToMe:
                        EmpowermentsToMeView()
                    case .empowermentsFromMe:
                        EmpowermentsFromMeView(path: $navPath)
                    case .createEmpowerment:
                        CreateEmpowermentView(viewModel: CreateEmpowermentViewModel(),
                                              path: $navPath)
                    case .logs:
                        LogsView()
                    case .faq:
                        FAQView()
                    case .contactUs:
                        ContactsView()
                    case .termsAndConditions:
                        TermsAndConditionsView()
                    case .administrators:
                        AdministratorsWebView()
                    case .centers:
                        CentersView()
                    case .providers:
                        ProvidersView()
                    case .secureDeliverySystem:
                        SecureDeliverySystemView()
                    case .paymentsHistory:
                        PaymentsHistoryView()
                    default:
                        EmptyView()
                    }
                } else {
                    EmptyView()
                }
            }
        }
    }
    
    // MARK: Child views
    private func menuButtonView(menuOption: MoreMenuOption) -> some View {
        HStack {
            Text(menuOption.localizedTitle())
                .font(.bodySmall)
                .lineSpacing(8)
                .foregroundStyle(Color.textWhite)
            Spacer()
            Image("icon_forward")
        }
    }
    
    private func onlineHelpMenuButton(menuOption: MoreMenuOption) -> some View {
        Button(action: {
            if let url = URL(string: EIDWebViewType.onlineHelpSystem.url) {
                if UIApplication.shared.canOpenURL(url) {
                    UIApplication.shared.open(url)
                }
            }
        }, label: {
            menuButtonView(menuOption: menuOption)
        })
    }
}
