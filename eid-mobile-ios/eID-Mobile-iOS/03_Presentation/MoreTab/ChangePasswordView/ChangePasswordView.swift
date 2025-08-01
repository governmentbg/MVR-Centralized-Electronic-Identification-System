//
//  ChangePasswordView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.05.24.
//

import SwiftUI


struct ChangePasswordView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @StateObject var viewModel = ChangePasswordViewModel()
    
    // MARK: - Body
    var body: some View {
        ScrollViewReader { reader in
            ScrollView {
                VStack(spacing: 32) {
                    oldPsswordField
                    newPasswordField
                    confirmNewPasswordField
                    Spacer()
                    changePasswordButton(reader: reader)
                }
            }
        }
        .padding()
        .addNavigationBar(title: MoreMenuOption.changePassword.localizedTitle(),
                          content: {
            ToolbarItem(placement: .topBarLeading) {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            }
        })
        .background(Color.themeSecondaryLight)
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {
            presentationMode.wrappedValue.dismiss()
        })
        .onTapGesture {
            hideKeyboard()
        }
    }
    
    // MARK: - Child views
    private var oldPsswordField: some View {
        EIDInputField(title: "change_password_old_password_title".localized(),
                      text: $viewModel.oldPassword.value,
                      showError: !$viewModel.oldPassword.validation.isValid,
                      errorText: $viewModel.oldPassword.validation.error,
                      isMandatory: true,
                      isPassword: true)
        .id(ChangePasswordViewModel.Field.oldPassword.rawValue)
    }
    
    private var newPasswordField: some View {
        EIDInputField(title: "change_password_new_password_title".localized(),
                      text: $viewModel.newPassword.value,
                      showError: !$viewModel.newPassword.validation.isValid,
                      errorText: $viewModel.newPassword.validation.error,
                      isMandatory: true,
                      isPassword: true)
        .id(ChangePasswordViewModel.Field.newPassword.rawValue)
    }
    
    private var confirmNewPasswordField: some View {
        EIDInputField(title: "change_password_confirm_new_password_title".localized(),
                      text: $viewModel.confirmNewPassword.value,
                      showError: !$viewModel.confirmNewPassword.validation.isValid,
                      errorText: $viewModel.confirmNewPassword.validation.error,
                      isMandatory: true,
                      isPassword: true)
        .id(ChangePasswordViewModel.Field.confirmNewPassword.rawValue)
    }
    
    private func changePasswordButton(reader: ScrollViewProxy) -> some View {
        Button(action: {
            if let errorField = viewModel.firstErrorField {
                reader.scrollTo(errorField.rawValue)
            }
            viewModel.changePassword()
        }, label: {
            Text("btn_confirm")
        })
        .buttonStyle(EIDButton())
    }
}

#Preview {
    ChangePasswordView()
}
