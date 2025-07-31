//
//  ChangeEmailView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.05.24.
//

import SwiftUI


struct ChangeEmailView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @StateObject var viewModel = ChangeEmailViewModel()
    // MARK: - Private Properties
    private var padding: CGFloat = 32
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: padding) {
            emailField
                .padding([.bottom], padding)
            buttons
            Spacer()
        }
        .contentShape(Rectangle())
        .padding()
        .addNavigationBar(title: MoreMenuOption.changeEmail.localizedTitle(),
                          content: {
            ToolbarItem(placement: .topBarLeading) {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            }
        })
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {
            presentationMode.wrappedValue.dismiss()
        })
        .background(Color.themeSecondaryLight)
        .onTapGesture {
            hideKeyboard()
        }
    }
    
    // MARK: - Child views
    private var emailField: some View {
        EIDInputField(title: "change_email_title".localized(),
                      text: $viewModel.email.value,
                      showError: !$viewModel.email.validation.isValid,
                      errorText: $viewModel.email.validation.error,
                      isMandatory: true,
                      keyboardType: .emailAddress,
                      submitLabel: .next,
                      autocapitalization: .never)
    }
    
    private var buttons: some View {
        Button(action: {
            viewModel.changeEmail()
        }, label: {
            Text("btn_confirm")
        })
        .buttonStyle(EIDButton())
    }
}

#Preview {
    ChangeEmailView()
}
