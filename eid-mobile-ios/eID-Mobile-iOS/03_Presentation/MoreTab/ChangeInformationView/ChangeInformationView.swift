//
//  ChangePhoneView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 19.06.24.
//

import SwiftUI


struct ChangeInformationView: View {
    
    // MARK: - Properties
    @StateObject var viewModel: ChangeInformationViewModel
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Private Properties
    private let padding: CGFloat = 32
    
    // MARK: - Body
    var body: some View {
        ScrollViewReader { reader in
            ScrollView {
                VStack(spacing: padding) {
                    firstNameField
                    secondNameField
                    lastNameField
                    firstNameLatinField
                    secondNameLatinField
                    lastNameLatinField
                    phoneNumberField
                    buttons(reader: reader)
                    Spacer()
                }
            }
        }
        .contentShape(Rectangle())
        .padding()
        .addNavigationBar(title: "change_information_title".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
            ToolbarItem(placement: .topBarTrailing) {
                Button(action: {
                    viewModel.showInfo = true
                }, label: {
                    Image("icon_info")
                        .renderingMode(.template)
                        .foregroundColor(Color.textWhite)
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
                         description: .constant("change_personal_info_description".localized()))
    }
    
    // MARK: - Child views
    private var firstNameField: some View {
        EIDNameField(title: "first_name_title".localized(),
                     name: $viewModel.firstName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     autocapitalization: .words,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(ChangeInformationViewModel.Field.firstName.rawValue)
    }
    
    private var secondNameField: some View {
        EIDNameField(title: "second_name_title".localized(),
                     name: $viewModel.secondName,
                     isMandatory: false,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(ChangeInformationViewModel.Field.secondName.rawValue)
    }
    
    private var lastNameField: some View {
        EIDNameField(title: "last_name_title".localized(),
                     name: $viewModel.lastName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.lastNameCharacterLimit)
        .id(ChangeInformationViewModel.Field.lastName.rawValue)
    }
    
    private var firstNameLatinField: some View {
        EIDNameField(title: "first_name_latin_title".localized(),
                     name: $viewModel.firstNameLatin,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     autocapitalization: .words,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(ChangeInformationViewModel.Field.firstNameLatin.rawValue)
    }
    
    private var secondNameLatinField: some View {
        EIDNameField(title: "second_name_latin_title".localized(),
                     name: $viewModel.secondNameLatin,
                     isMandatory: false,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .id(ChangeInformationViewModel.Field.secondNameLatin.rawValue)
    }
    
    private var lastNameLatinField: some View {
        EIDNameField(title: "last_name_latin_title".localized(),
                     name: $viewModel.lastNameLatin,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.lastNameCharacterLimit)
        .id(ChangeInformationViewModel.Field.lastNameLatin.rawValue)
    }
    
    private var phoneNumberField: some View {
        EIDPhoneField(phone: $viewModel.phone,
                      title: "phone_title".localized(),
                      shouldValidate: viewModel.shouldValidateForm)
        .id(ChangeInformationViewModel.Field.phone.rawValue)
        .padding([.bottom], padding)
    }
    
    private func buttons(reader: ScrollViewProxy) -> some View {
        Button(action: {
            if let errorField = viewModel.firstErrorField {
                reader.scrollTo(errorField.rawValue)
            }
            viewModel.changePhone()
        }, label: {
            Text("btn_confirm")
        })
        .buttonStyle(EIDButton())
    }
}

#Preview {
    ChangeInformationView(
        viewModel: ChangeInformationViewModel(
            citizenEid: nil
        )
    )
}
