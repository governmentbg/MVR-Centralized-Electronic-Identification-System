//
//  RegisterView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.05.24.
//

import SwiftUI


struct RegisterView: View {
    // MARK: - Properties
    @StateObject var viewModel = RegisterViewModel()
    @Environment(\.presentationMode) var presentationMode
    private var verticalPadding: CGFloat = 32
    private var horizontalPadding: CGFloat = 24
    
    // MARK: - Body
    var body: some View {
        ScrollViewReader { reader in
            ScrollView {
                VStack(spacing: verticalPadding) {
                    titleView
                    DisclaimerMessageView(title: "register_view_disclaimer_title".localized())
                    firstNameField
                    secondNameField
                    lastNameField
                    firstNameLatinField
                    secondNameLatinField
                    lastNameLatinField
                    emailField
                    EIDPhoneField(phone: $viewModel.phone,
                                  title: "application_details_phone_number_title".localized(),
                                  shouldValidate: viewModel.shouldValidateForm)
                    passwordField
                    repeatPasswordField
                    registerButton(reader: reader)
                }
                .padding([.leading, .trailing], horizontalPadding)
            }
            .scrollDismissesKeyboard(.interactively)
        }
        .setBackground()
        .addTransparentGradientDividerNavigationBar(content: {
            ToolbarItem(placement: .topBarLeading) {
                EIDBackButton()
            }
            ToolbarItem(placement: .topBarTrailing) {
                Button(action: {
                    viewModel.showInfo = true
                }, label: {
                    Image("icon_info")
                        .renderingMode(.template)
                        .foregroundColor(Color.buttonDefault)
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
        .presentInfoView(showInfo: $viewModel.showInfo,
                         title: .constant("info_title".localized()),
                         description: .constant("register_info_description".localized()))
    }
    
    // MARK: - Child views
    private var titleView: some View {
        Text("register_view_title".localized().uppercased())
            .font(.bodyLarge)
            .lineSpacing(8)
            .foregroundStyle(Color.textActive)
            .padding([.top], verticalPadding)
    }
    
    // MARK: - Form Views
    private var firstNameField: some View {
        EIDNameField(title: "first_name_title".localized(),
                     name: $viewModel.firstName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     autocapitalization: .words,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(RegisterViewModel.Field.firstName.rawValue)
    }
    
    private var secondNameField: some View {
        EIDNameField(title: "second_name_title".localized(),
                     name: $viewModel.secondName,
                     isMandatory: false,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(RegisterViewModel.Field.secondName.rawValue)
    }
    
    private var lastNameField: some View {
        EIDNameField(title: "last_name_title".localized(),
                     name: $viewModel.lastName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.lastNameCharacterLimit)
        .id(RegisterViewModel.Field.lastName.rawValue)
    }
    
    private var firstNameLatinField: some View {
        EIDNameField(title: "first_name_latin_title".localized(),
                     name: $viewModel.firstNameLatin,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     autocapitalization: .words,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(RegisterViewModel.Field.firstNameLatin.rawValue)
    }
    
    private var secondNameLatinField: some View {
        EIDNameField(title: "second_name_latin_title".localized(),
                     name: $viewModel.secondNameLatin,
                     isMandatory: false,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(RegisterViewModel.Field.secondNameLatin.rawValue)
    }
    
    private var lastNameLatinField: some View {
        EIDNameField(title: "last_name_latin_title".localized(),
                     name: $viewModel.lastNameLatin,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.lastNameCharacterLimit)
        .id(RegisterViewModel.Field.lastNameLatin.rawValue)
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
        .id(RegisterViewModel.Field.email.rawValue)
    }
    
    private var passwordField: some View {
        EIDInputField(title: "login_title_password".localized(),
                      text: $viewModel.password.value,
                      showError: !$viewModel.password.validation.isValid,
                      errorText: $viewModel.password.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      isMandatory: true,
                      isPassword: true)
        .id(RegisterViewModel.Field.password.rawValue)
    }
    
    private var repeatPasswordField: some View {
        EIDInputField(title: "register_repeat_password".localized(),
                      text: $viewModel.repeatPassword.value,
                      showError: !$viewModel.repeatPassword.validation.isValid,
                      errorText: $viewModel.repeatPassword.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      isMandatory: true,
                      isPassword: true)
        .id(RegisterViewModel.Field.repeatPassword.rawValue)
    }
    
    private func registerButton(reader: ScrollViewProxy) -> some View {
        Button(action: {
            if let errorField = viewModel.firstErrorField {
                reader.scrollTo(errorField.rawValue)
            }
            viewModel.register()
        }, label: {
            Text("btn_create_profile".localized())
        })
        .buttonStyle(EIDButton())
    }
}

#Preview {
    RegisterView()
}
