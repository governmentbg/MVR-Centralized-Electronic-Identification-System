//
//  LoginView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.10.23.
//

import SwiftUI


struct LoginView: View {
    // MARK: - Properties
    @StateObject var viewModel = LoginViewModel()
    @EnvironmentObject private var appRootManager: AppRootManager
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        VStack() {
            Spacer()
            EIDTitleView()
            Spacer()
            Spacer()
            loginForm
            forgottenPasswordButton
                .padding([.bottom], 8)
        }
        .addTransparentGradientDividerNavigationBar(content: {
            ToolbarItem(placement: .topBarLeading, content: {
                EIDBackButton()
            })
        })
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .setBackground()
        .onAppear {
            viewModel.clearFields()
            viewModel.observeUnauthorized()
        }
        .onTapGesture {
            hideKeyboard()
        }
        .navigationDestination(isPresented: $viewModel.showTwoFactorAuthView) {
            MultifactorAuthenticationView(twoFactorResponse: viewModel.twoFactorResponse)
        }
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
    }
    
    // MARK: - Child views
    private var loginForm: some View {
        VStack(spacing: 24) {
            EIDInputField(title: "application_details_email_title".localized(),
                          text: $viewModel.email.value,
                          showError: !$viewModel.email.validation.isValid,
                          errorText: $viewModel.email.validation.error,
                          shouldValidate: viewModel.shouldValidateForm,
                          isMandatory: true,
                          submitLabel: .next,
                          autocapitalization: .never)
            EIDInputField(title: "login_title_password".localized(),
                          text: $viewModel.password.value,
                          showError: !$viewModel.password.validation.isValid,
                          errorText: $viewModel.password.validation.error,
                          shouldValidate: viewModel.shouldValidateForm,
                          isMandatory: true,
                          isPassword: true)
            Button(action: {
                sendLoginRequest()
            }, label: {
                Text("btn_login".localized())
            })
            .buttonStyle(EIDButton())
        }
        .padding([.leading, .trailing], 38)
        .padding([.bottom], 24)
    }
    
    private var forgottenPasswordButton: some View {
        NavigationLink(destination: ForgottenPasswordView()) {
            Text("btn_forgotten_password".localized())
                .font(.bodySmall)
                .foregroundStyle(Color.textActive)
        }
    }
    
    // MARK: - Helpers
    private func sendLoginRequest() {
        viewModel.getAuthToken(onSuccess: {
            withAnimation(.spring()) {
                appRootManager.currentRoot = .home
            }
        }, onFailure: {})
    }
}


// MARK: - Preview
#Preview {
    LoginView()
}
