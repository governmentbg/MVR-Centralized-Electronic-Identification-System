//
//  ApplicationsFilterView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.03.24.
//

import SwiftUI


struct ApplicationsFilterView: View {
    // MARK: - Properties
    @Binding var administrators: [EIDAdministrator]
    @Binding var devices: [EIDDevice]
    /// Filter data to send
    @Binding var status: ApplicationStatus?
    @Binding var id: String
    @Binding var applicationNumber: String
    @Binding var createDateText: String
    @Binding var deviceId: String?
    @Binding var applicationType: EIDApplicationType?
    @Binding var selectedAdministrator: EIDAdministrator
    /// Fields
    @State private var statusText: String = "option_all".localized()
    @State private var deviceName: String = "option_all".localized()
    @State private var applicationTypeText: String = "option_all".localized()
    /// Callbacks
    var applyFilter: () -> Void
    var dismiss: () -> Void
    /// Private
    @State private var administratorItems: [PickerViewItem] = []
    @State private var createDate: Date = .now
    @State private var showCreateDatePicker: Bool = false
    @State private var showAdministratorsList: Bool = false
    @FocusState private var focusState: Bool
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            Divider()
            ScrollView {
                VStack(spacing: 24) {
                    applicationNumberField
                    createDateField
                    statusField
                    //                    identifierField
                    applicationTypeField
                    deviceField
                    administratorField
                }
                .padding()
            }
            Divider()
            filterButtons
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.backgroundWhite)
        .onAppear {
            setupFields()
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
    
    /// Status
    private var statusField: some View {
        Menu(content: {
            Button(action: {
                hideKeyboard()
                setStatus(nil)
            }, label: {
                Text("option_all".localized())
            })
            ForEach(ApplicationStatus.filterStatuses, id: \.self) { applicationStatus in
                Button(action: {
                    hideKeyboard()
                    setStatus(applicationStatus)
                }, label: {
                    Text(applicationStatus.title.localized())
                })
            }
        }, label: {
            EIDInputField(title: "application_status_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $statusText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    /// ID
    private var identifierField: some View {
        EIDInputField(title: "identifier_type_title".localized(),
                      text: $id,
                      showError: .constant(false),
                      errorText: .constant(""))
    }
    
    /// Application number
    private var applicationNumberField: some View {
        EIDInputField(title: "application_number_title".localized(),
                      text: $applicationNumber,
                      showError: .constant(false),
                      errorText: .constant(""))
    }
    
    /// Create date
    private var createDateField: some View {
        EIDInputField(title: "application_sort_criteria_create_date".localized(),
                      text: $createDateText,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showCreateDatePicker = true
        })
        .showDatePicker(showPicker: $showCreateDatePicker,
                        selectedDate: $createDate,
                        rangeThrough: ...Date(),
                        title: "application_sort_criteria_create_date".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setCreateDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearCreateDate() })
    }
    
    /// Administrator
    private var administratorField: some View {
        EIDInputField(title: "application_administrator_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $selectedAdministrator.name,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .arrowDown,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showAdministratorsList.toggle()
        })
        .sheet(isPresented: $showAdministratorsList,
               content: {
            EIDSearchablePickerView(title: "application_administrator_title".localized(),
                                    items: administratorItems,
                                    selection: $selectedAdministrator.id,
                                    didSelectItem: { administrator in
                selectedAdministrator = administrators.first(where: { $0.id == administrator.id }) ?? .default
            })
        })
    }
    
    /// Device
    private var deviceField: some View {
        Menu(content: {
            Button(action: {
                hideKeyboard()
                setDevice(nil)
            }, label: {
                Text("option_all".localized())
            })
            ForEach(devices, id: \.self) { device in
                Button(action: {
                    hideKeyboard()
                    setDevice(device.id)
                }, label: {
                    Text(device.name)
                })
            }
        }, label: {
            EIDInputField(title: "application_carrier_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $deviceName,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    /// Application type
    private var applicationTypeField: some View {
        Menu(content: {
            Button(action: {
                hideKeyboard()
                setApplicationType(nil)
            }, label: {
                Text("option_all".localized())
            })
            ForEach(EIDApplicationType.allCases, id: \.self) { type in
                Button(action: {
                    hideKeyboard()
                    setApplicationType(type)
                }, label: {
                    Text(type.title.localized())
                })
            }
        }, label: {
            EIDInputField(title: "application_type_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $applicationTypeText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
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
                applyFilter()
            }, label: {
                Text("btn_apply_filters".localized())
            })
            .buttonStyle(EIDButton())
        }
        .padding()
        .background(Color.themeSecondaryLight)
    }
    
    // MARK: - Helpers
    private func clearFilter() {
        id = ""
        applicationNumber = ""
        setStatus(nil)
        selectedAdministrator = .default
        setApplicationType(nil)
        setDevice(nil)
        clearCreateDate()
        applyFilter()
    }
    
    private func setupFields() {
        setAdministratorItems()
        statusText = status?.title.localized() ?? "option_all".localized()
        setDevice(deviceId)
        applicationTypeText = applicationType?.title.localized() ?? "option_all".localized()
        if let date = createDateText.toDate(withFormats: [.iso8601Date]) {
            createDate = date
            setCreateDate()
        } else {
            clearCreateDate()
        }
    }
    
    private func setAdministratorItems() {
        administratorItems.removeAll()
        administratorItems.append(PickerViewItem(id: "", name: "option_all".localized()))
        let administratorsToAdd = administrators.map { PickerViewItem(id: $0.id, name: $0.name) }
        administratorItems.append(contentsOf: administratorsToAdd)
    }
    
    private func setStatus(_ newValue: ApplicationStatus?) {
        status = newValue
        statusText = status?.title.localized() ?? "option_all".localized()
    }
    
    private func setDevice(_ newValue: String?) {
        deviceId = newValue
        deviceName = devices.first(where: { $0.id == deviceId })?.name ?? "option_all".localized()
    }
    
    private func clearCreateDate() {
        createDate = .now
        createDateText = ""
    }
    
    private func setCreateDate() {
        createDateText = createDate.normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func setApplicationType(_ newValue: EIDApplicationType?) {
        applicationType = newValue
        applicationTypeText = applicationType?.title.localized() ?? "option_all".localized()
    }
}
