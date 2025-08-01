//
//  MainLoginView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 31.08.23.
//

import SwiftUI


struct MainLoginView: View {
    // MARK: - Properties
    @EnvironmentObject private var appRootManager: AppRootManager
    @StateObject private var viewModel = MainLoginViewModel()
    
    // MARK: - Body
    var body: some View {
        NavigationStack {
            VStack() {
                Spacer()
                EIDTitleView()
                Spacer()
                Spacer()
                VStack(spacing: 32) {
                    loginWithPasswordButton
                    if CertificateManager.hasMobileEidStored {
                        loginWithMobileEidButton
                    }
                    loginWithCardButton
                    createProfileButton
                }
                .padding([.leading, .trailing], 40)
            }
            .navigationDestination(isPresented: $viewModel.handleAction) {
                switch viewModel.selectedAction {
                case .goToLoginView:
                    LoginView()
                case .goToRegisterView:
                    RegisterView()
                case .goToTwoFactorView:
                    MultifactorAuthenticationView(twoFactorResponse: viewModel.twoFactorResponse)
                case .goTologinWithCardView:
                    EIDPINView(viewModel: EIDPINViewModel(state: .loginWithCard,
                                                          onPINAuth: { succeess in
                        if succeess {
                            viewModel.didFinishCardLoginSuccessfully = true
                            if viewModel.didReceiveNFCCardSessionEnd {
                                goToHome(delay: false)
                            }
                        }
                    }))
                case .goTologinWithMobileEidView:
                    EIDPINView(viewModel: EIDPINViewModel(state: .loginWithMobileEID,
                                                          onPINAuth: { succeess in
                        if succeess {
                            goToHome(delay: true)
                        }
                    }))
                case .noAction:
                    EmptyView()
                }
            }
            .addTransparentGradientDividerNavigationBar(content: {
                if AppConfiguration.currentEnvironment() != .production {
                    environmentSelectButton
                }
                LanguageMenu(showLanguageAlert: $viewModel.showLanguageAlert)
            })
            .presentMultipleButtonAlert(showAlert: $viewModel.showLanguageAlert,
                                        alertText: $viewModel.languageAlertTitle,
                                        buttons: viewModel.languageAlertButtons)
            .onAppear {
                hideKeyboard()
            }
            .onReceive(NotificationCenter.default.publisher(for: .nfcSheetWasDismissed), perform: { _ in
                viewModel.didReceiveNFCCardSessionEnd = true
                if viewModel.didFinishCardLoginSuccessfully {
                    goToHome(delay: false)
                }
            })
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .setBackground()
            .sheet(isPresented: $viewModel.showLoginWithPINView, content: {
                LoginWithDevicePINView(viewModel: EIDPINViewModel(state: .loginWithDevicePIN,
                                                                  pin: viewModel.pin,
                                                                  onPINCreate: { didMatch, _ in
                    if didMatch {
                        viewModel.getAuthToken(onSuccess: {
                            goToHome(delay: false)
                        }, onFailure: {})
                    } else {
                        viewModel.showLoginWithPINView = false
                        appRootManager.logoutUser()
                        DispatchQueue.main.asyncAfter(deadline: .now() + 0.3) {
                            viewModel.selectedAction = .goToLoginView
                            viewModel.handleAction = true
                        }
                    }
                }))
            })
            .observeLoading(isLoading: $viewModel.showLoading)
            .presentAlert(showAlert: $viewModel.showError,
                          alertText: $viewModel.errorText)
        }
    }
    
    // MARK: - Child views
    private var loginWithPasswordButton: some View {
        Button(action: {
            viewModel.loginAction {
                goToHome(delay: false)
            }
        }, label: {
            Text(viewModel.buttonTitle)
        })
        .buttonStyle(EIDButton(buttonState: .primary))
    }
    
    private var loginWithMobileEidButton: some View {
        Button(action: {
            viewModel.selectedAction = .goTologinWithMobileEidView
            viewModel.handleAction = true
        }, label: {
            Text("btn_login_with_eid".localized())
        })
        .buttonStyle(EIDButton(buttonState: .primary))
    }
    
    private var loginWithCardButton: some View {
        Button(action: {
            viewModel.selectedAction = .goTologinWithCardView
            viewModel.handleAction = true
        }, label: {
            Text("btn_login_with_identity_card_eID".localized())
        })
        .buttonStyle(EIDButton(buttonState: .primary))
    }
    
    private var createProfileButton: some View {
        Button(action: {
            viewModel.selectedAction = .goToRegisterView
            viewModel.handleAction = true
        }, label: {
            Text("btn_create_profile".localized())
        })
        .buttonStyle(EIDButton(buttonState: .primary))
    }
    
    private var environmentSelectButton: some ToolbarContent {
        ToolbarItem(placement: .topBarLeading, content: {
            Menu(content: {
                ForEach(AppEnvironment.selectOptions, id: \.self) { env in
                    Button(action: {
                        hideKeyboard()
                        viewModel.setEnv(env)
                    }, label: {
                        HStack {
                            Text(env.rawValue)
                            if viewModel.currentEnv == env {
                                Image("icon_selected_blue")
                            }
                        }
                    })
                }
            }, label: {
                HStack {
                    Image("icon_settings")
                    Text(viewModel.currentEnv.rawValue)
                        .font(.extraTiny)
                        .lineSpacing(4)
                        .foregroundStyle(Color.textInactive)
                }
            })
            .preferredColorScheme(.light)
        })
    }
    
    // MARK: - Helpers
    func getCurrentEnv() -> AppEnvironment {
        return AppConfiguration.currentEnvironment()
    }
    
    func goToHome(delay: Bool) {
        DispatchQueue.main.asyncAfter(deadline: .now() + (delay ? 0.5 : 0.0)) {
            withAnimation(.spring()) {
                appRootManager.currentRoot = .home
            }
        }
    }
}


// MARK: - Preview
#Preview {
    MainLoginView()
}
