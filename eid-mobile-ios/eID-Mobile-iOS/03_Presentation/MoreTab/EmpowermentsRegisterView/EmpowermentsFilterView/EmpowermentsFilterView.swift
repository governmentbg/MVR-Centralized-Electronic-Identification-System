//
//  EmpowermentsFilterView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 22.11.23.
//

import SwiftUI


struct EmpowermentsFilterView: View {
    // MARK: - Properties
    var empowermentDirection: EmpowermentDirection
    /// Filter data to send
    @Binding var number: String
    @Binding var status: EmpowermentStatus?
    @Binding var onBehalfOf: EmpowermentOnBehalfOf?
    @Binding var authorizer: String
    @Binding var providerName: String
    @Binding var serviceName: String
    @Binding var showOnlyNoExpiryDate: Bool
    @Binding var validToDateText: String
    @Binding var empoweredUids: [UserIdentifier]
    @Binding var eik: String
    /// Initial values backup in case of dismiss
    @State private var initialStatus: EmpowermentStatus? = nil
    @State private var initialOnBehalfOf: EmpowermentOnBehalfOf? = nil
    @State private var initialAuthorizer: String = ""
    @State private var initialProviderName: String = ""
    @State private var initialServiceName: String = ""
    @State private var initialShowOnlyNoExpiryDate: Bool = false
    @State private var initialValidToDateText: String = ""
    @State private var initialEmpoweredUids: [UserIdentifier] = []
    /// Fields
    @State private var statusText: String = "option_all".localized()
    @State private var onBehalfOfText: String = ""
    /// Callbacks
    var applyFilter: () -> Void
    var dismiss: () -> Void
    /// Private
    @State private var shouldApplyNewValues = false
    @State private var validToDate: Date = .now
    @State private var showValidToDatePicker: Bool = false
    @State private var showAddENGButton: Bool = true
    @FocusState private var focusState: Bool
    @State var authorizedIds: [PersonalID] = []
    private var authorizedIdFields: [PersonalIDField] {
        return setAuthorizedIdFields()
    }
    /// Computed helper properties
    private var visibleEgnFields: [PersonalID] {
        return authorizedIds.filter({$0.visible})
    }
    private var hiddenEgnFields: [PersonalID] {
        return authorizedIds.filter({!$0.visible})
    }
    private var invalidIds: [String] {
        return authorizedIds.filter({ !$0.id.isEmpty && !$0.validation.isValid }).map({ $0.id })
    }
    private var verticalPadding: CGFloat {
        return 24
    }
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            Divider()
            ScrollView {
                VStack(spacing: 0) {
                    VStack(spacing: verticalPadding) {
                        numberFilters
                        statusFilters
                        if empowermentDirection != .fromMeEIK {
                            authorizerFilters
                        }
                        serviceFilters
                        validityFilters
                        if empowermentDirection != .toMe {
                            dynamicENGFieldsWithButton
                        }
                    }
                    .padding()
                    Divider()
                    filterButtons
                }
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.backgroundWhite)
        .onAppear {
            setupFields()
        }
        .onDisappear {
            if !shouldApplyNewValues {
                loadInitialValues()
            }
        }
        .onTapGesture {
            hideKeyboard()
        }
    }
    
    // MARK: - Child views
    private var titleView: some View {
        HStack {
            Text("filter_title".localized().uppercased())
                .font(.heading4)
                .foregroundStyle(Color.textDefault)
            Spacer()
            Button(action: {
                dismiss()
            }, label: {
                Image("icon_cross_dark")
            })
        }
        .padding()
        .frame(maxWidth: .infinity)
        .background(Color.themeSecondaryLight)
    }
    
    /// Number fields
    private var numberFilters: some View {
        VStack(alignment: .leading, spacing: verticalPadding) {
            Text("empowerments_number_title".localized())
                .font(.bodyLarge)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            numberField
        }
    }
    
    private var numberField: some View {
        EIDInputField(title: "empowerments_number_field_title".localized(),
                      hint: "hint_please_enter".localized(),
                      text: $number,
                      showError: .constant(false),
                      errorText: .constant(""))
    }
    
    /// Status fields
    private var statusFilters: some View {
        VStack(alignment: .leading, spacing: verticalPadding) {
            Text("empowerments_status_title".localized())
                .font(.bodyLarge)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            statusField
        }
    }
    
    private var statusField: some View {
        Menu(content: {
            Button(action: {
                setStatus(nil)
            }, label: {
                Text("option_all".localized())
            })
            ForEach(empowermentDirection.filterStatuses, id: \.self) { empowermentStatus in
                Button(action: {
                    setStatus(empowermentStatus)
                }, label: {
                    Text(empowermentStatus.title.localized())
                })
            }
        }, label: {
            EIDInputField(title: "empowerments_status_field_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $statusText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    /// Authorizer fields
    private var authorizerFilters: some View {
        VStack(alignment: .leading, spacing: verticalPadding) {
            Text("empowerment_authorizer_title".localized())
                .font(.bodyLarge)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            onBehalfOfField
            
            switch onBehalfOf {
            case .empty, .individual:
                authorizerField
            case .legalEntity:
                legalEntityNameField
                legalEntityEikField
            default:
                EmptyView()
            }
        }
    }
    
    private var onBehalfOfField: some View {
        Menu(content: {
            Button(action: {
                setOnBehalfOf(.empty)
            }, label: {
                Text("option_all".localized())
            })
            Button(action: {
                setOnBehalfOf(EmpowermentOnBehalfOf.individual)
            }, label: {
                Text(EmpowermentOnBehalfOf.individual.title.localized())
            })
            Button(action: {
                setOnBehalfOf(EmpowermentOnBehalfOf.legalEntity)
            }, label: {
                Text(EmpowermentOnBehalfOf.legalEntity.title.localized())
            })
        }, label: {
            EIDInputField(title: "empowerments_from_me_on_behalf_of_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $onBehalfOfText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    private var legalEntityNameField: some View {
        EIDInputField(title: "empowerment_from_legal_entity_name_title".localized(),
                      hint: "hint_please_enter".localized(),
                      text: $authorizer,
                      showError: .constant(false),
                      errorText: .constant(""))
    }
    
    /// EIK field
    private var legalEntityEikField: some View {
        EIDInputField(title: "empowerment_eik_search_title".localized(),
                      hint: "hint_please_enter".localized(),
                      text: $eik,
                      showError: .constant(false),
                      errorText: .constant(""),
                      keyboardType: .numberPad)
    }
    
    private var authorizerField: some View {
        EIDInputField(title: "empowerment_authorizer_title".localized(),
                      hint: "hint_please_enter".localized(),
                      text: $authorizer,
                      showError: .constant(false),
                      errorText: .constant(""))
    }
    
    /// Service fields
    private var serviceFilters: some View {
        VStack(alignment: .leading, spacing: verticalPadding) {
            Text("empowerment_service_title".localized())
                .font(.bodyLarge)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            providerField
            serviceField
        }
    }
    
    private var providerField: some View {
        EIDInputField(title: "empowerment_provider_title".localized(),
                      hint: "empowerment_provider_hint".localized(),
                      text: $providerName,
                      showError: .constant(false),
                      errorText: .constant(""))
        .onSubmit {
            if providerName.isEmpty {
                serviceName = ""
            }
        }
    }
    
    private var serviceField: some View {
        EIDInputField(title: "empowerment_service_title".localized(),
                      hint: "empowerment_service_hint".localized(),
                      text: $serviceName,
                      showError: .constant(false),
                      errorText: .constant(""))
        .disabled(providerName.isEmpty)
    }
    
    /// Validity fields
    private var validityFilters: some View {
        VStack(alignment: .leading, spacing: verticalPadding) {
            Text("empowerment_validity_title".localized())
                .font(.bodyLarge)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            validToDateField
            noExpiryCheckbox
                .padding([.top], -verticalPadding/2)
        }
    }
    
    private var validToDateField: some View {
        EIDInputField(title: "empowerment_valid_to_date_title".localized(),
                      hint: "empowerment_valid_to_date_hint".localized(),
                      text: $validToDateText,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showValidToDatePicker = true
        })
        .showDatePicker(showPicker: $showValidToDatePicker,
                        selectedDate: $validToDate,
                        rangeFrom: nil,
                        title: "empowerment_valid_to_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setValidToDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearValidToDate() })
    }
    
    private var noExpiryCheckbox: some View {
        Toggle(isOn: $showOnlyNoExpiryDate) {
            Text("empowerment_expiry_date_indefinitely".localized())
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textLight)
        }
        .toggleStyle(CheckboxToggleStyle(action: {
            if showOnlyNoExpiryDate {
                clearValidToDate()
            }
        }))
    }
    
    /// EGN fields
    private var dynamicENGFieldsWithButton: some View {
        VStack(alignment: .leading, spacing: verticalPadding) {
            Text("empowerment_section_empowered_ids_title".localized())
                .font(.bodyLarge)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            ForEach(authorizedIdFields.indices, id: \.self) { index in
                if authorizedIdFields[index].personalId.visible {
                    authorizedIdFields[index].idTypeField
                        .focused($focusState)
                    authorizedIdFields[index].idField
                        .focused($focusState)
                }
            }
            if showAddENGButton {
                Button(action: {
                    focusState = false
                    addENGField()
                }, label: {
                    Text("btn_empowerment_add_empowered_id".localized())
                })
                .buttonStyle(EIDButton(buttonType: .text,
                                       buttonState: .success))
            }
        }
    }
    
    /// Buttons
    private var filterButtons: some View {
        HStack {
            Button(action: {
                clearFilter()
            }, label: {
                Text("btn_clear".localized())
            })
            .buttonStyle(EIDButton(buttonState: .danger))
            Button(action: {
                if empowermentDirection != .toMe {
                    setEmpoweredUids()
                }
                submitFields()
            }, label: {
                Text("btn_apply_filters".localized())
            })
            .buttonStyle(EIDButton())
            .disabled(!invalidIds.isEmpty)
        }
        .padding()
        .background(Color.themeSecondaryLight)
    }
    
    // MARK: - Helpers
    private func clearFilter() {
        setStatus(nil)
        setOnBehalfOf(.empty)
        providerName = ""
        serviceName = ""
        number = ""
        eik = ""
        authorizedIds = []
        empoweredUids = []
        clearValidToDate()
        showOnlyNoExpiryDate = false
        submitFields()
    }
    
    private func setupFields() {
        if onBehalfOf == nil {
            setOnBehalfOf(.empty)
        }
        statusText = status?.title.localized() ?? "option_all".localized()
        onBehalfOfText = onBehalfOf?.title.localized() ?? "option_all".localized()
        if let date = validToDateText.toDate(withFormats: [.iso8601Date]) {
            validToDate = date
            setValidToDate()
        } else {
            clearValidToDate()
        }
        initAuthorizedIds()
        initialStatus = status
        initialOnBehalfOf = .empty
        initialAuthorizer = authorizer
        initialProviderName = providerName
        initialServiceName = serviceName
        initialShowOnlyNoExpiryDate = showOnlyNoExpiryDate
        initialValidToDateText = validToDateText
        initialEmpoweredUids = empoweredUids
    }
    
    private func loadInitialValues() {
        status = initialStatus
        onBehalfOf = initialOnBehalfOf
        authorizer = initialAuthorizer
        providerName = initialProviderName
        serviceName = initialServiceName
        showOnlyNoExpiryDate = initialShowOnlyNoExpiryDate
        validToDateText = initialValidToDateText
        empoweredUids = initialEmpoweredUids
    }
    
    private func submitFields() {
        empoweredUids = empoweredUids.filter({ $0.uid?.isEmpty == false })
        shouldApplyNewValues = true
        applyFilter()
    }
    
    private func setStatus(_ newValue: EmpowermentStatus?) {
        status = newValue
        statusText = status?.title.localized() ?? "option_all".localized()
    }
    
    private func setOnBehalfOf(_ newValue: EmpowermentOnBehalfOf?) {
        onBehalfOf = newValue
        onBehalfOfText = onBehalfOf?.title.localized() ?? "option_all".localized()
        authorizer = ""
    }
    
    private func clearValidToDate() {
        validToDate = .now
        validToDateText = ""
    }
    
    private func setValidToDate() {
        validToDateText = validToDate.normalizeDate(outputFormat: .iso8601Date)
        showOnlyNoExpiryDate = false
    }
    
    private func initAuthorizedIds() {
        var ids: [PersonalID] = []
        for empoweredUid in empoweredUids {
            ids.append(PersonalID(idType: empoweredUid.uidType ?? .egn,
                                  idTypeText: empoweredUid.uidType?.rawValue ?? "",
                                  id: empoweredUid.uid ?? "",
                                  name: Name(value: "", isLatin: false),
                                  visible: true))
        }
        for _ in empoweredUids.count...9 {
            ids.append(PersonalID(idType: .egn,
                                  idTypeText: IdentifierType.egn.title.localized(),
                                  id: "",
                                  name: Name(value: "", isLatin: false),
                                  visible: false))
        }
        ids[0].visible = true // set first field visible if empoweredUids is empty
        authorizedIds = ids
    }
    
    private func setEmpoweredUids() {
        empoweredUids = visibleEgnFields.filter({ $0.validation.isValid }).map({ UserIdentifier(uid: $0.id, uidType: $0.idType)})
    }
    
    private func setAuthorizedIdFields() -> [PersonalIDField] {
        var fields: [PersonalIDField] = []
        for (index, _) in authorizedIds.enumerated() {
            let idTypeField = Menu(content: {
                Button(action: {
                    setIdFieldType(.egn, index: index)
                }, label: {
                    Text(IdentifierType.egn.title.localized())
                })
                Button(action: {
                    setIdFieldType(.lnch, index: index)
                }, label: {
                    Text(IdentifierType.lnch.title.localized())
                })
            }, label: {
                EIDInputField(title: "identifier_type_title".localized(),
                              hint: "hint_please_select".localized(),
                              text: $authorizedIds[index].idTypeText,
                              showError: .constant(false),
                              errorText: .constant(""),
                              rightIcon: .arrowDown)
            })
            fields.append(PersonalIDField(personalId: $authorizedIds[index],
                                          idTypeField: AnyView(idTypeField),
                                          idField: EIDInputField(title: "empowerment_to_id_title".localized(),
                                                                 hint: "hint_please_enter".localized(),
                                                                 text: $authorizedIds[index].id,
                                                                 showError: !$authorizedIds[index].validation.isValid,
                                                                 errorText: $authorizedIds[index].validation.error,
                                                                 rightIcon: authorizedIds.filter({$0.visible}).count > 1 ? .cross : .none,
                                                                 rightIconAction: { removeENGField(index: index) },
                                                                 keyboardType: .numberPad)))
        }
        return fields
    }
    
    private func addENGField() {
        for (index, field) in authorizedIds.enumerated() {
            if !field.visible {
                authorizedIds[index].visible = true
                break
            }
        }
        showAddENGButton = !hiddenEgnFields.isEmpty
    }
    
    private func setIdFieldType(_ idType: IdentifierType, index: Int) {
        guard index < authorizedIds.count else { return }
        authorizedIds[index].id = ""
        authorizedIds[index].idType = idType
    }
    
    private func removeENGField(index: Int) {
        guard index < authorizedIds.count else { return }
        authorizedIds[index].id = ""
        authorizedIds[index].visible = false
        showAddENGButton = !hiddenEgnFields.isEmpty
    }
}
