//
//  ForgottenPasswordView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.05.24.
//

import SwiftUI


struct ForgottenPasswordView: View {
    // MARK: - Properties
    @StateObject var viewModel = ForgottenPasswordViewModel()
    @Environment(\.presentationMode) var presentationMode
    private var verticalPadding: CGFloat = 32
    private var horizontalPadding: CGFloat = 24
    
    // MARK: - Body
    var body: some View {
        ScrollViewReader { reader in
            ScrollView {
                VStack(spacing: verticalPadding) {
                    titleView
                    DisclaimerMessageView(title: "forgotten_password_disclaimer_title".localized())
                    firstNameField
                    secondNameField
                    lastNameField
                    emailField
                    Spacer()
                    button(reader: reader)
                }
                .padding([.leading, .trailing], horizontalPadding)
            }
            .scrollDismissesKeyboard(.interactively)
        }
        .setBackground()
        .addTransparentGradientDividerNavigationBar(content: {
            ToolbarItem(placement: .topBarLeading, content: {
                EIDBackButton()
            })
        })
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
    private var titleView: some View {
        Text("forgotten_password_title".localized().uppercased())
            .font(.bodyLarge)
            .lineSpacing(8)
            .foregroundStyle(Color.textActive)
            .padding([.top], verticalPadding)
    }
    
    private var firstNameField: some View {
        EIDNameField(title: "first_name_title".localized(),
                     name: $viewModel.firstName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     autocapitalization: .words,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(ForgottenPasswordViewModel.Field.firstName.rawValue)
    }
    
    private var secondNameField: some View {
        EIDNameField(title: "second_name_title".localized(),
                     name: $viewModel.secondName,
                     isMandatory: false,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(ForgottenPasswordViewModel.Field.secondName.rawValue)
    }
    
    private var lastNameField: some View {
        EIDNameField(title: "last_name_title".localized(),
                     name: $viewModel.lastName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.lastNameCharacterLimit)
        .id(ForgottenPasswordViewModel.Field.lastName.rawValue)
    }
    
    private var emailField: some View {
        EIDInputField(title: "application_details_email_title".localized(),
                      text: $viewModel.email.value,
                      showError: !$viewModel.email.validation.isValid,
                      errorText: $viewModel.email.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      isMandatory: true,
                      keyboardType: .emailAddress,
                      submitLabel: .next,
                      autocapitalization: .never)
        .id(ForgottenPasswordViewModel.Field.email.rawValue)
    }
    
    private func button(reader: ScrollViewProxy) -> some View {
        Button(action: {
            if let errorField = viewModel.firstErrorField {
                reader.scrollTo(errorField.rawValue)
            }
            viewModel.submit()
        }, label: {
            Text("btn_change_password".localized())
        })
        .buttonStyle(EIDButton())
    }
}

#Preview {
    ForgottenPasswordView()
}
