//
//  CreateApplicationView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 1.03.24.
//

import SwiftUI


struct CreateApplicationView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    var certificateActionsInfo: CertificateDetailsActionModel?
    var viewState: CertificateDetailsViewModel.ViewState?
    var reasons: [Reason] = []
    @StateObject var viewModel: CreateApplicationViewModel
    @FocusState private var focusState: Bool
    @State var scrollToTop = false
    @State private var menuRefreshID = UUID()
    @Binding var path: [String]
    private var identityFields: PersonalIDField {
        return setPersonalIdFields()
    }
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            GradientDivider()
            ScrollViewReader { reader in
                ScrollView {
                    VStack(spacing: 24) {
                        namesView
                        if viewModel.shouldShowReasonField {
                            reasonsField
                        }
                        personalInfoSection
                        if !viewModel.formState.isCertificateState {
                            additionalInfoSection
                        }
                        if viewModel.formState.shouldSignCertificateChange {
                            signatureProviderField
                        }
                        Spacer()
                        switch viewModel.formState {
                        case .edit:
                            editStateButtons(reader: reader)
                        case .preview:
                            previewStateButtons
                        case .stopCertificate, .resumeCertificate, .revokeCertificate:
                            certificateActionStateButtons(reader: reader)
                        }
                    }
                    .padding()
                }
                .scrollToTopHandler(scrollOnChange: $scrollToTop)
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .navigationDestination(isPresented: $viewModel.showPaymentView) {
            ApplicationPaymentView(path: $path,
                                   payment: viewModel.applicationPayment)
        }
        .addNavigationBar(title: viewModel.screenTitle.localized(),
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
        .observeLoading(isLoading: $viewModel.showSigningLoading, blurRadius: 4, details: String(format: "signing_loading_title".localized(), viewModel.signatureProvider.appName))
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText,
                      onDismiss: {
            viewModel.showLoading = false
        })
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {
            if viewModel.formState.isCertificateState {
                path.removeAll()
            } else {
                presentationMode.wrappedValue.dismiss()
            }
        })
        .onAppear {
            if let certificateActionsInfo = certificateActionsInfo, let viewState = viewState {
                viewModel.certificateActionsInfo = certificateActionsInfo
                viewModel.formState = viewState.createApplicationFormState
                viewModel.reasons = reasons
            }
            viewModel.getInitalData()
            if viewModel.applicationPayment != nil {
                presentationMode.wrappedValue.dismiss()
            }
        }
        .onTapGesture {
            hideKeyboard()
        }
    }
    
    // MARK: - Child views
    private var titleView: some View {
        Text(viewModel.viewTitle)
            .font(.heading4)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .multilineTextAlignment(.center)
            .padding()
    }
    
    private var namesView: some View {
        VStack {
            Text(viewModel.fullName)
                .font(.bodyLarge)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.center)
                .padding([.bottom], 8)
            Text(viewModel.email)
                .font(.bodySmall)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.center)
        }
        .id("topScrollPoint")
    }
    
    /// Sections
    private var personalInfoSection: some View {
        VStack(alignment: .leading, spacing: 24) {
            personalDocumentSectionTitleView
            //            firstNameField
            //            secondNameField
            //            lastNameField
            firstNameLatinField
            secondNameLatinField
            lastNameLatinField
            citizenshipField
            citizenIdentifierFields
            dateOfBirthField
            personalDocumentSection
        }
    }
    
    private var personalDocumentSection: some View {
        VStack(alignment: .leading, spacing: 24) {
            identityTypeField
            identityNumberField
            identityIssueDateField
            identityValidToDateField
        }
    }
    
    private var additionalInfoSection: some View {
        VStack(alignment: .leading, spacing: 24) {
            additionalInfoSectionTitleView
            administratorField
            deviceField
            officeField
            commentField
            if UserManager.getUser()?.acr != .high {
                signatureProviderField
            }
        }
    }
    
    /// Section titles
    private var personalDocumentSectionTitleView: some View {
        Text("application_section_personal_document_title".localized().uppercased())
            .font(.heading6)
            .lineSpacing(10)
            .foregroundStyle(Color.textDefault)
    }
    
    private var additionalInfoSectionTitleView: some View {
        Text("application_section_additional_info_title".localized().uppercased())
            .font(.heading6)
            .lineSpacing(10)
            .foregroundStyle(Color.textDefault)
    }
    
    /// Stop/Resume Certificate
    private var reasonsField: some View {
        return VStack(spacing: 16) {
            Menu(content: {
                ForEach(viewModel.reasons, id: \.self) { reason in
                    Button(action: {
                        hideKeyboard()
                        viewModel.setReason(reason.description, isOtherSelected: reason.textRequired)
                    }, label: {
                        Text(reason.description)
                    })
                }
            }, label: {
                EIDInputField(title: "empowerment_reason_title".localized(),
                              hint: "hint_please_select".localized(),
                              text: $viewModel.reason.value,
                              showError: !$viewModel.reason.validation.isValid,
                              errorText: $viewModel.reason.validation.error,
                              shouldValidate: viewModel.shouldValidateForm,
                              rightIcon: .arrowDown,
                              isMandatory: true)
                .disabled(true)
            })
            .preferredColorScheme(.light)
            if viewModel.showCustomReason {
                EIDInputField(title: "common_label_description".localized(),
                              hint: "hint_please_enter".localized(),
                              text: $viewModel.customReason.value,
                              showError: !$viewModel.customReason.validation.isValid,
                              errorText: $viewModel.customReason.validation.error,
                              shouldValidate: viewModel.shouldValidateForm,
                              isMandatory: true)
            }
        }
    }
    
    /// Base User
    private var firstNameField: some View {
        EIDNameField(title: "first_name_title".localized(),
                     name: $viewModel.firstName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     autocapitalization: .words,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .disabled(viewModel.formState == .preview)
    }
    
    private var secondNameField: some View {
        EIDNameField(title: "second_name_title".localized(),
                     name: $viewModel.secondName,
                     isMandatory: false,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .disabled(viewModel.formState == .preview)
    }
    
    private var lastNameField: some View {
        EIDNameField(title: "last_name_title".localized(),
                     name: $viewModel.lastName,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.lastNameCharacterLimit)
        .disabled(viewModel.formState == .preview)
    }
    
    private var firstNameLatinField: some View {
        EIDNameField(title: "first_name_latin_title".localized(),
                     name: $viewModel.firstNameLatin,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     autocapitalization: .words,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.firstNameLatin.rawValue)
    }
    
    private var secondNameLatinField: some View {
        EIDNameField(title: "second_name_latin_title".localized(),
                     name: $viewModel.secondNameLatin,
                     isMandatory: false,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.nameCharacterLimit)
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.secondNameLatin.rawValue)
    }
    
    private var lastNameLatinField: some View {
        EIDNameField(title: "last_name_latin_title".localized(),
                     name: $viewModel.lastNameLatin,
                     isMandatory: true,
                     shouldValidate: viewModel.shouldValidateForm,
                     characterLimit: Constants.CharacterLimits.lastNameCharacterLimit)
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.lastNameLatin.rawValue)
    }
    
    private var dateOfBirthField: some View {
        EIDInputField(title: "date_of_birth_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.dateOfBirth.value,
                      showError: !$viewModel.dateOfBirth.validation.isValid,
                      errorText: $viewModel.dateOfBirth.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            viewModel.showBirthDatePicker = true
            hideKeyboard()
        },
                      isMandatory: true)
        .showDatePicker(showPicker: $viewModel.showBirthDatePicker,
                        selectedDate: $viewModel.birthDate,
                        rangeThrough: ...Date(),
                        title: "date_of_birth_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { viewModel.setBirthDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { viewModel.clearBirthDate() })
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.dateOfBirth.rawValue)
    }
    
    private var citizenIdentifierFields: some View {
        VStack(spacing: 24) {
            identityFields.idTypeField
                .disabled(viewModel.formState == .preview)
                .id(CreateApplicationViewModel.Field.citizenIdentifierType.rawValue)
            identityFields.idField
                .disabled(viewModel.formState == .preview)
                .id(CreateApplicationViewModel.Field.citizenIdentifier.rawValue)
        }
    }
    
    private var citizenshipField: some View {
        EIDInputField(title: "citizenship_title".localized(),
                      text: $viewModel.citizenship.value,
                      showError: !$viewModel.citizenship.validation.isValid,
                      errorText: $viewModel.citizenship.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      isMandatory: true)
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.citizenship.rawValue)
    }
    
    private var identityNumberField: some View {
        EIDInputField(title: "document_number_title".localized(),
                      text: $viewModel.identityNumber.value,
                      showError: !$viewModel.identityNumber.validation.isValid,
                      errorText: $viewModel.identityNumber.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      isMandatory: true,
                      autocapitalization: .characters,
                      textCase: .uppercase)
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.identityNumber.rawValue)
    }
    
    private var identityTypeField: some View {
        Menu(content: {
            ForEach(viewModel.identityTypes, id: \.self) { identityType in
                Button(action: {
                    hideKeyboard()
                    viewModel.identityType = identityType
                }, label: {
                    Text(identityType.title.localized())
                })
            }
        }, label: {
            EIDInputField(title: "document_type_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $viewModel.identityTypeStr.value,
                          showError: !$viewModel.identityTypeStr.validation.isValid,
                          errorText: $viewModel.identityTypeStr.validation.error,
                          shouldValidate: viewModel.shouldValidateForm,
                          rightIcon: .arrowDown,
                          isMandatory: true)
        })
        .preferredColorScheme(.light)
        .disabled(viewModel.formState == .preview)
    }
    
    private var identityIssueDateField: some View {
        EIDInputField(title: "identity_issue_date_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.identityIssueDate.value,
                      showError: !$viewModel.identityIssueDate.validation.isValid,
                      errorText: $viewModel.identityIssueDate.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            viewModel.showIssueDatePicker = true
            hideKeyboard()
        },
                      isMandatory: true)
        .showDatePicker(showPicker: $viewModel.showIssueDatePicker,
                        selectedDate: $viewModel.issueDate,
                        rangeThrough: ...Date(),
                        title: "identity_issue_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { viewModel.setIssueDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { viewModel.clearIssueDate() })
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.identityIssueDate.rawValue)
    }
    
    private var identityValidToDateField: some View {
        EIDInputField(title: "identity_valid_to_date_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.identityValidityToDate.value,
                      showError: !$viewModel.identityValidityToDate.validation.isValid,
                      errorText: $viewModel.identityValidityToDate.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            viewModel.showValidToDatePicker = true
            hideKeyboard()
        },
                      isMandatory: true)
        .showDatePicker(showPicker: $viewModel.showValidToDatePicker,
                        selectedDate: $viewModel.validToDate,
                        rangeFrom: Date()...,
                        title: "identity_valid_to_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { viewModel.setValidToDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { viewModel.clearValidToDate() })
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.identityValidityToDate.rawValue)
    }
    
    /// Common
    private var administratorField: some View {
        EIDInputField(title: "create_application_administrator_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.selectedAdministrator.name,
                      showError: !$viewModel.selectedAdministrator.validation.isValid,
                      errorText: $viewModel.selectedAdministrator.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .arrowDown,
                      isPicker: true,
                      tapAction: {
            focusState = false
            viewModel.showAdministratorsList.toggle()
            hideKeyboard()
        },
                      isMandatory: true)
        .sheet(isPresented: $viewModel.showAdministratorsList,
               content: {
            EIDSearchablePickerView(title: "create_application_administrator_title".localized(),
                                    items: viewModel.administratorItems,
                                    selection: $viewModel.selectedAdministrator.id,
                                    didSelectItem: { item in
                viewModel.selectedAdministrator = viewModel.administrators.first(where: { $0.id == item.id }) ?? .default
            })
        })
        .disabled(viewModel.formState == .preview)
        .id(CreateApplicationViewModel.Field.selectedAdministrator.rawValue)
    }
    
    private var deviceField: some View {
        ZStack {
            Menu(content: {
                ForEach(viewModel.deviceItems, id: \.self) { device in
                    Button(action: {
                        hideKeyboard()
                        viewModel.selectedDevice = device
                    }, label: {
                        Text(device.name)
                    })
                }
            }, label: {
                EIDInputField(title: "application_carrier_title".localized(),
                              hint: "hint_please_select".localized(),
                              text: $viewModel.selectedDevice.name,
                              showError: !$viewModel.selectedDevice.validation.isValid,
                              errorText: $viewModel.selectedDevice.validation.error,
                              shouldValidate: viewModel.shouldValidateForm,
                              rightIcon: .arrowDown,
                              isMandatory: true)
            })
            .id(CreateApplicationViewModel.Field.selectedDevice.rawValue)
            .preferredColorScheme(.light)
        }
        .id(viewModel.deviceItems.count)
        .disabled(viewModel.selectedAdministrator.validation.isNotValid || viewModel.formState == .preview)
    }
    
    private var officeField: some View {
        EIDInputField(title: "application_administrator_office_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.selectedOffice.name,
                      showError: !$viewModel.selectedOffice.validation.isValid,
                      errorText: $viewModel.selectedOffice.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .arrowDown,
                      isPicker: true,
                      tapAction: {
            focusState = false
            viewModel.showOfficesList.toggle()
            hideKeyboard()
        },
                      isMandatory: true)
        .sheet(isPresented: $viewModel.showOfficesList,
               content: {
            EIDSearchablePickerView(title: "empowerment_service_title".localized(),
                                    items: viewModel.officeItems,
                                    selection: $viewModel.selectedOffice.id, didSelectItem: { item in
                viewModel.selectedOffice = viewModel.offices.first(where: { $0.id == item.id }) ?? .default
            })
        })
        .disabled(viewModel.selectedDevice.validation.isNotValid || viewModel.selectedAdministrator.validation.isNotValid || viewModel.formState == .preview || viewModel.isOnlineOffice)
        .id(CreateApplicationViewModel.Field.selectedOffice.rawValue)
    }
    
    private var commentField: some View {
        VStack(alignment: .leading, spacing: 4) {
            Text("application_comment_title".localized())
                .font(.label)
                .lineSpacing(8)
                .foregroundStyle(.textLight)
                .multilineTextAlignment(.leading)
            Text("application_comment".localized())
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.leading)
            
        }
        .frame(maxWidth: .infinity, alignment: .leading)
    }
    
    private var signatureProviderField: some View {
        VStack(spacing: 16) {
            Menu(content: {
                Button(action: {
                    hideKeyboard()
                    viewModel.setSignatureProvider(.borica)
                }, label: {
                    Text(SignatureProvider.borica.title.localized())
                })
                Button(action: {
                    hideKeyboard()
                    viewModel.setSignatureProvider(.evrotrust)
                }, label: {
                    Text(SignatureProvider.evrotrust.title.localized())
                })
            }, label: {
                EIDInputField(title: "create_application_signing_method".localized(),
                              hint: "hint_please_select".localized(),
                              text: $viewModel.signatureProviderText.value,
                              showError: !$viewModel.signatureProviderText.validation.isValid,
                              errorText: $viewModel.signatureProviderText.validation.error,
                              shouldValidate: viewModel.shouldValidateForm,
                              rightIcon: .arrowDown,
                              isMandatory: true)
                .disabled(viewModel.formState == .preview)
            })
            .preferredColorScheme(.light)
            .disabled(viewModel.formState == .preview)
        }
    }
    
    private func editStateButtons(reader: ScrollViewProxy) -> some View {
        VStack {
            Button(action: {
                if let errorField = viewModel.firstErrorField {
                    reader.scrollTo(errorField.rawValue)
                }
                viewModel.validateAndPreview()
                scrollToTop.toggle()
            }, label: {
                Text("btn_preview".localized())
            })
            .buttonStyle(EIDButton())
            
            Button(action: {
                presentationMode.wrappedValue.dismiss()
            }, label: {
                Text("btn_cancel".localized())
            })
            .buttonStyle(EIDButton(buttonType: .filled,
                                   buttonState: .danger))
        }
    }
    
    private var previewStateButtons: some View {
        VStack {
            Button(action: {
                viewModel.submitApplication()
            }, label: {
                Text(viewModel.previewStateButtonTitle)
            })
            .buttonStyle(EIDButton())
            
            Button(action: {
                viewModel.changeState(to: .edit)
            }, label: {
                Text("btn_edit".localized())
            })
            .buttonStyle(EIDButton(buttonType: .outline))
        }
    }
    
    private func certificateActionStateButtons(reader: ScrollViewProxy) -> some View {
        VStack {
            Button(action: {
                presentationMode.wrappedValue.dismiss()
            }, label: {
                Text("btn_back".localized())
            })
            .buttonStyle(EIDButton(buttonType: .outline))
            
            Button(action: {
                if let errorField = viewModel.firstErrorField {
                    reader.scrollTo(errorField.rawValue)
                }
                viewModel.changeCertificateStatus()
            }, label: {
                Text(viewModel.certificateActionButtonTitle)
            })
            .buttonStyle(EIDButton())
        }
    }
    
    // MARK: - Helpers
    private func setPersonalIdFields() -> PersonalIDField {
        let idTypeField = Menu(content: {
            Button(action: {
                hideKeyboard()
                viewModel.setIdFieldType(.egn)
            }, label: {
                Text(IdentifierType.egn.title.localized())
            })
            Button(action: {
                hideKeyboard()
                viewModel.setIdFieldType(.lnch)
            }, label: {
                Text(IdentifierType.lnch.title.localized())
            })
        }, label: {
            EIDInputField(title: "identifier_type_field_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $viewModel.citizenIdentifier.idTypeText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          shouldValidate: viewModel.shouldValidateForm,
                          rightIcon: .arrowDown,
                          isMandatory: true)
        })
        return PersonalIDField(personalId: $viewModel.citizenIdentifier,
                               idTypeField: AnyView(idTypeField),
                               idField: EIDInputField(title: "identifier_type_title".localized(),
                                                      hint: "hint_please_enter".localized(),
                                                      text: $viewModel.citizenIdentifier.id,
                                                      showError: !$viewModel.citizenIdentifier.validation.isValid,
                                                      errorText: $viewModel.citizenIdentifier.validation.error,
                                                      shouldValidate: viewModel.shouldValidateForm,
                                                      isMandatory: true,
                                                      keyboardType: .numberPad))
    }
}


// MARK: - Preview
#Preview {
    CreateApplicationView(viewModel: CreateApplicationViewModel(administrators: [],
                                                                devices: []),
                          path: .constant([""]))
}
