//
//  CreateEmpowermentView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 31.10.23.
//

import SwiftUI


struct CreateEmpowermentView: View {
    // MARK: - Properties
    @StateObject var viewModel: CreateEmpowermentViewModel
    @Environment(\.presentationMode) var presentationMode
    @State var scrollToTop = false
    @Binding var path: [String]
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            GradientDivider()
            ScrollViewReader { reader in
                ScrollView {
                    VStack(spacing: 24) {
                        authorizerSection
                        if viewModel.isExpanded {
                            if viewModel.authorizerType == .legalEntity {
                                legalAuthorizersSection
                            }
                            empoweredIdsSection
                            empowermentSection
                        }
                        if viewModel.formState == .edit {
                            editStateButtons(reader: reader)
                        } else {
                            previewStateButtons
                        }
                    }
                    .padding()
                }
                .scrollToTopHandler(scrollOnChange: $scrollToTop)
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "empowerments_screen_title".localized(),
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
            path.removeLast()
            DispatchQueue.main.asyncAfter(deadline: .now() + 0.5) {
                path.append(MoreMenuDestinations.empowermentsFromMe.rawValue)
            }
        })
        .presentInfoView(showInfo: $viewModel.showInfo,
                         title: .constant("info_title".localized()),
                         description: $viewModel.infoText)
        .sheet(isPresented: $viewModel.showIndefiniteAlert, content: {
            IndefiniteEmpowermentSheetView {
                viewModel.showIndefiniteAlert = false
                viewModel.changeState(to: .preview)
            }
        })
        .onTapGesture {
            hideKeyboard()
        }
        .onChange(of: viewModel.formState) { newValue in
            if newValue == .preview {
                scrollToTop.toggle()
            }
        }
    }
    
    // MARK: - Child views
    /// Sections
    private var authorizerSection: some View {
        VStack(alignment: .leading, spacing: 24) {
            sectionTitle("empowerment_section_authorizer_title".localized())
            authorizerTypeField
            if viewModel.isExpanded {
                if viewModel.authorizerType == .individual {
                    authorizerIdTypeField
                    authorizerIdField
                } else {
                    legalEntityFields
                }
            }
        }
        .id("topScrollPoint")
    }
    
    private var legalAuthorizersSection: some View {
        VStack(alignment: .leading, spacing: 24) {
            sectionTitle("empowerment_section_legal_representatives_title".localized())
            dynamicAuthorizerFields
        }
    }
    
    private var empoweredIdsSection: some View {
        VStack(alignment: .leading, spacing: 24) {
            sectionTitle("empowerment_section_empowered_ids_title".localized())
            dynamicEmpoweredFields
        }
    }
    
    private var empowermentSection: some View {
        VStack(alignment: .leading, spacing: 24) {
            sectionTitle("empowerment_section_empowerment_title".localized())
            providerField
            serviceField
            scopeField
            startDateField
            endDateField
        }
    }
    
    /// Titles
    private var titleView: some View {
        Text(viewModel.formState == .edit ? "empowerment_title_edit".localized() : "empowerment_title_preview".localized())
            .font(.heading4)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .padding()
            .id("topScrollPoint")
    }
    
    private func sectionTitle(_ title: String) -> some View {
        Text(title)
            .font(.heading6Bold)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
    }
    
    /// Authorizer type - Individual or Legal entity
    private var authorizerTypeField: some View {
        Menu(content: {
            Button(action: {
                hideKeyboard()
                viewModel.setAuthorizerType(.individual)
            }, label: {
                Text(EmpowermentOnBehalfOf.individual.title.localized())
            })
            Button(action: {
                hideKeyboard()
                viewModel.setAuthorizerType(.legalEntity)
            }, label: {
                Text(EmpowermentOnBehalfOf.legalEntity.title.localized())
            })
        }, label: {
            EIDInputField(title: "empowerments_from_me_on_behalf_of_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $viewModel.authorizerTypeText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown,
                          isMandatory: true)
            .disabled(viewModel.formState == .preview)
        })
        .preferredColorScheme(.light)
        .disabled(viewModel.formState == .preview)
    }
    
    /// Individual authorizer - ID type and ID
    private var authorizerIdTypeField: some View {
        EIDInputField(title: "identifier_type_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.authorizerIdTypeText,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .arrowDown,
                      isMandatory: true)
        .disabled(true)
    }
    
    private var authorizerIdField: some View {
        EIDInputField(title: "empowerment_personal_number_field_title".localized(),
                      hint: "hint_please_enter".localized(),
                      text: $viewModel.authorizerId.value,
                      showError: !$viewModel.authorizerId.validation.isValid,
                      errorText: $viewModel.authorizerId.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      isMandatory: true)
        .disabled(true)
    }
    
    /// Legal entity info
    private var legalEntityFields: some View {
        VStack(spacing: 24) {
            EIDInputField(title: "empowerment_from_bulstat_title".localized(),
                          hint: "hint_please_enter".localized(),
                          text: $viewModel.authorizerBulstat.value,
                          showError: !$viewModel.authorizerBulstat.validation.isValid,
                          errorText: $viewModel.authorizerBulstat.validation.error,
                          shouldValidate: viewModel.shouldValidateForm,
                          isMandatory: true,
                          keyboardType: .numberPad)
            .disabled(viewModel.formState == .preview)
            .id(CreateEmpowermentViewModel.Field.authorizerBulstat.rawValue)
            EIDInputField(title: "empowerment_from_legal_entity_name_title".localized(),
                          hint: "hint_please_enter".localized(),
                          text: $viewModel.authorizerName.value,
                          showError: !$viewModel.authorizerName.validation.isValid,
                          errorText: $viewModel.authorizerName.validation.error,
                          shouldValidate: viewModel.shouldValidateForm,
                          isMandatory: true)
            .disabled(viewModel.formState == .preview)
            .id(CreateEmpowermentViewModel.Field.authorizerName.rawValue)
            EIDInputField(title: "empowerment_from_role_title".localized(),
                          hint: "hint_please_enter".localized(),
                          text: $viewModel.authorizerRole.value,
                          showError: !$viewModel.authorizerRole.validation.isValid,
                          errorText: $viewModel.authorizerRole.validation.error,
                          shouldValidate: viewModel.shouldValidateForm,
                          isMandatory: true)
            .disabled(viewModel.formState == .preview)
            .id(CreateEmpowermentViewModel.Field.authorizerRole.rawValue)
        }
    }
    
    /// Authorizer IDs
    private var dynamicAuthorizerFields: some View {
        VStack(spacing: 24) {
            ForEach(viewModel.authorizerIds.indices, id: \.self) { index in
                let authorizer = viewModel.authorizerIds[index]
                let field = personalIDField(id: $viewModel.authorizerIds[index], index: index, isAuthorizer: true)
                field.idTypeField
                    .disabled(viewModel.formState == .preview || index == 0)
                field.idField
                    .id(authorizer.fieldId)
                    .disabled(viewModel.formState == .preview || index == 0)
                field.nameField
                    .id(authorizer.nameFieldId)
                    .disabled(viewModel.formState == .preview || index == 0)
            }
            
            if viewModel.showAddAuthorizerIdButton {
                Button(action: {
                    hideKeyboard()
                    viewModel.addNewIdField(isAuthorizer: true)
                }, label: {
                    Text("btn_empowerment_add_authorizer".localized())
                })
                .buttonStyle(EIDButton(buttonType: .text,
                                       buttonState: .success))
            }
        }
    }
    
    /// Empowered IDs
    private var dynamicEmpoweredFields: some View {
        VStack(spacing: 24) {
            ForEach(viewModel.empoweredIds.indices, id: \.self) { index in
                let empowered = viewModel.empoweredIds[index]
                let field = personalIDField(id: $viewModel.empoweredIds[index], index: index, isAuthorizer: false)
                field.idTypeField
                    .disabled(viewModel.formState == .preview)
                field.idField
                    .id(empowered.fieldId)
                    .disabled(viewModel.formState == .preview)
                field.nameField
                    .id(empowered.nameFieldId)
                    .disabled(viewModel.formState == .preview)
            }
            Button(action: {
                hideKeyboard()
                viewModel.addNewIdField(isAuthorizer: false)
            }, label: {
                Text("btn_empowerment_add_empowered_id".localized())
            })
            .buttonStyle(EIDButton(buttonType: .text,
                                   buttonState: .success))
            if viewModel.showEmpowermentField {
                empowermentTypeField
            }
        }
    }
    
    /// Type: Together or separately
    private var empowermentTypeField: some View {
        Menu(content: {
            Button(action: {
                hideKeyboard()
                viewModel.setEmpowermentType(.separately)
            }, label: {
                Text(EmpowermentType.separately.title.localized())
            })
            Button(action: {
                hideKeyboard()
                viewModel.setEmpowermentType(.togetherOnly)
            }, label: {
                Text(EmpowermentType.togetherOnly.title.localized())
            })
        }, label: {
            EIDInputField(title: "empowerment_type_field_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $viewModel.empowermentTypeStr.value,
                          showError: !$viewModel.empowermentTypeStr.validation.isValid,
                          errorText: $viewModel.empowermentTypeStr.validation.error,
                          shouldValidate: viewModel.shouldValidateForm,
                          rightIcon: .arrowDown,
                          isMandatory: true)
            .disabled(viewModel.formState == .preview)
        })
        .preferredColorScheme(.light)
        .disabled(viewModel.formState == .preview)
    }
    
    /// Provider
    private var providerField: some View {
        EIDInputField(title: "empowerment_provider_field_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.providerName.value,
                      showError: !$viewModel.providerName.validation.isValid,
                      errorText: $viewModel.providerName.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .arrowDown,
                      isPicker: true,
                      tapAction: {
            hideKeyboard()
            viewModel.showProvidersList.toggle()
        },
                      isMandatory: true)
        .sheet(isPresented: $viewModel.showProvidersList,
               content: {
            EIDSearchablePickerView(title: "empowerment_provider_field_title".localized(),
                                    items: viewModel.providerItems,
                                    selection: $viewModel.providerId)
        })
        .disabled(viewModel.formState == .preview)
        .id(CreateEmpowermentViewModel.Field.selectedProvider.rawValue)
    }
    
    /// Service
    private var serviceField: some View {
        EIDInputField(title: "empowerment_service_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.serviceName.value,
                      showError: !$viewModel.serviceName.validation.isValid,
                      errorText: $viewModel.serviceName.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .arrowDown,
                      isPicker: true,
                      tapAction: {
            hideKeyboard()
            viewModel.showServicesList.toggle()
        },
                      isMandatory: true)
        .sheet(isPresented: $viewModel.showServicesList,
               content: {
            EIDSearchablePickerView(title: "empowerment_service_title".localized(),
                                    items: viewModel.serviceItems,
                                    selection: $viewModel.serviceId)
        })
        .disabled(viewModel.providerId.isEmpty || viewModel.formState == .preview)
        .id(CreateEmpowermentViewModel.Field.selectedService.rawValue)
    }
    
    /// Scope
    private var scopeField: some View {
        EIDInputField(title: "empowerment_scope_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.scopeName.value,
                      showError: !$viewModel.scopeName.validation.isValid,
                      errorText: $viewModel.scopeName.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .arrowDown,
                      isPicker: true,
                      tapAction: {
            hideKeyboard()
            viewModel.showScopeList.toggle()
        },
                      isMandatory: true)
        .sheet(isPresented: $viewModel.showScopeList,
               content: {
            EIDSearchableMultiPickerView(title: "empowerment_scope_title".localized(),
                                         items: $viewModel.scopeItems,
                                         allSelected: $viewModel.allScopeSelected,
                                         onSelectionChange: { viewModel.scopeChanged() })
        })
        .disabled(viewModel.serviceId.isEmpty || viewModel.formState == .preview)
        .id(CreateEmpowermentViewModel.Field.selectedScope.rawValue)
    }
    
    /// Dates
    private var startDateField: some View {
        EIDInputField(title: "logs_start_date_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.startDateStr.value,
                      showError: !$viewModel.startDateStr.validation.isValid,
                      errorText: $viewModel.startDateStr.validation.error,
                      shouldValidate: viewModel.shouldValidateForm,
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            hideKeyboard()
            viewModel.showStartDatePicker = true
        },
                      isMandatory: true)
        .showDatePicker(showPicker: $viewModel.showStartDatePicker,
                        selectedDate: $viewModel.startDate,
                        rangeFrom: Date()...,
                        title: "logs_start_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { viewModel.setStartDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { viewModel.clearStartDate() })
        .disabled(viewModel.formState == .preview)
        .id(CreateEmpowermentViewModel.Field.startDate.rawValue)
    }
    
    private var endDateField: some View {
        EIDInputField(title: "logs_еnd_date_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $viewModel.endDateStr.value,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            hideKeyboard()
            viewModel.showEndDatePicker = true
        })
        .showDatePicker(showPicker: $viewModel.showEndDatePicker,
                        selectedDate: $viewModel.endDate,
                        rangeFrom: viewModel.startDate...,
                        title: "logs_start_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { viewModel.setEndDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { viewModel.clearEndDate() })
        .disabled(viewModel.formState == .preview)
    }
    
    /// Buttons
    private func editStateButtons(reader: ScrollViewProxy) -> some View {
        VStack {
            if viewModel.authorizerType != nil {
                Button(action: {
                    if let errorField = viewModel.firstErrorField {
                        switch errorField {
                        case .empoweredIds, .authorizerIds:
                            handleDynamicFocusFieldError(field: errorField, reader: reader)
                        default:
                            reader.scrollTo(errorField)
                        }
                    }
                    viewModel.checkEmpowermentValidity()
                }, label: {
                    Text("btn_preview".localized())
                })
                .buttonStyle(EIDButton())
            }
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
                viewModel.createEmpowerment()
            }, label: {
                Text("btn_submit".localized())
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
    
    // MARK: - Helpers
    private func personalIDField(id: Binding<PersonalID>, index: Int, isAuthorizer: Bool) -> PersonalIDField {
        let idTypeField = Menu(content: {
            Button(action: {
                hideKeyboard()
                viewModel.setIdFieldType(.egn, index: index, isAuthorizer: isAuthorizer)
            }, label: {
                Text(IdentifierType.egn.title.localized())
            })
            Button(action: {
                hideKeyboard()
                viewModel.setIdFieldType(.lnch, index: index, isAuthorizer: isAuthorizer)
            }, label: {
                Text(IdentifierType.lnch.title.localized())
            })
        }, label: {
            EIDInputField(title: "identifier_type_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: id.idTypeText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown,
                          isMandatory: true)
        })
        
        return PersonalIDField(personalId: id,
                               idTypeField: AnyView(idTypeField),
                               idField: EIDInputField(title: "empowerment_to_id_title".localized(),
                                                      hint: "hint_please_enter".localized(),
                                                      text: id.id,
                                                      showError: !id.validation.isValid,
                                                      errorText: id.validation.error,
                                                      shouldValidate: viewModel.shouldValidateForm,
                                                      rightIcon: (isAuthorizer ? viewModel.authorizerIds : viewModel.empoweredIds).count > 1 ? .cross : .none,
                                                      rightIconAction: { viewModel.removeIdField(index: index, isAuthorizer: isAuthorizer) },
                                                      isMandatory: true,
                                                      keyboardType: .numberPad,
                                                      focusChanged: { focused in
            if !focused {
                isAuthorizer ? viewModel.validateAuthorizers() : viewModel.validateEmpowerers()
            }
        }),
                               nameField: EIDNameField(title: "empowerment_names_title".localized(),
                                                       name: id.name,
                                                       isMandatory: true,
                                                       shouldValidate: viewModel.shouldValidateForm,
                                                       hint: "hint_please_enter".localized(),
                                                       rightIcon: (isAuthorizer ? viewModel.authorizerIds : viewModel.empoweredIds).count > 1 ? .cross : .none,
                                                       rightIconAction: { viewModel.removeIdField(index: index, isAuthorizer: isAuthorizer) }))
    }
    
    private func handleDynamicFocusFieldError(field: CreateEmpowermentViewModel.Field, reader: ScrollViewProxy) {
        if let personalId = viewModel.inputFieldValue(field: field) as? PersonalID {
            let errorFieldId = personalId.idIsNotValid ? personalId.fieldId : personalId.nameFieldId
            reader.scrollTo(errorFieldId)
        }
    }
}


// MARK: - Preview
#Preview {
    CreateEmpowermentView(viewModel: CreateEmpowermentViewModel(),
                          path: .constant([""]))
}
