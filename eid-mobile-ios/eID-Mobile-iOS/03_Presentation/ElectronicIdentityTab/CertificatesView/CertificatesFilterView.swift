//
//  CertificatesFilterView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.04.24.
//

import SwiftUI


struct CertificatesFilterView: View {
    // MARK: - Properties
    @Binding var administrators: [EIDAdministrator]
    @Binding var devices: [EIDDevice]
    /// Filter data to send
    @Binding var status: CertificateStatus?
    @Binding var serialNumber: String
    @Binding var validFromDateStr: String
    @Binding var validUntilDateStr: String
    @Binding var deviceId: String?
    @Binding var certificateName: String
    @Binding var selectedAdministrator: EIDAdministrator
    /// Fields
    @State private var statusText: String = "option_all".localized()
    @State private var deviceName: String = "option_all".localized()
    /// Callbacks
    var applyFilter: () -> Void
    var dismiss: () -> Void
    /// Private
    @State private var administratorItems: [PickerViewItem] = []
    @State private var validFromDate: Date = .now
    @State private var validUntilDate: Date = .now
    @State private var showAdministratorsList: Bool = false
    @State private var showValidFromDatePicker: Bool = false
    @State private var showValidUntilDatePicker: Bool = false
    @FocusState private var focusState: Bool
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            Divider()
            ScrollView {
                VStack(spacing: 24) {
                    serialNumberField
                    certificateNameField
                    validFromDateField
                    validUntilDateField
                    statusField
                    carrierField
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
            ForEach(CertificateStatus.filterStatuses, id: \.self) { certificateStatus in
                Button(action: {
                    hideKeyboard()
                    setStatus(certificateStatus)
                }, label: {
                    Text(certificateStatus.title.localized())
                })
            }
        }, label: {
            EIDInputField(title: "certificate_status_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $statusText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    /// Serial number
    private var serialNumberField: some View {
        EIDInputField(title: "certificate_serial_number_title".localized(),
                      text: $serialNumber,
                      showError: .constant(false),
                      errorText: .constant(""))
    }
    
    /// Valid from date
    private var validFromDateField: some View {
        EIDInputField(title: "identity_issue_date_title".localized(),
                      text: $validFromDateStr,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showValidFromDatePicker = true
        })
        .showDatePicker(showPicker: $showValidFromDatePicker,
                        selectedDate: $validFromDate,
                        rangeThrough: ...Date(),
                        title: "certificate_valid_from_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setValidFromDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearValidFromDate() })
    }
    
    /// Valid until date
    private var validUntilDateField: some View {
        EIDInputField(title: "certificate_valid_until_title".localized(),
                      text: $validUntilDateStr,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showValidUntilDatePicker = true
        })
        .showDatePicker(showPicker: $showValidUntilDatePicker,
                        selectedDate: $validUntilDate,
                        range: nil,
                        title: "certificate_valid_until_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setValidUntilDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearValidUntilDate() })
    }
    
    /// Certificate name
    private var certificateNameField: some View {
        EIDInputField(title: "certificate_name_title".localized(),
                      text: $certificateName,
                      showError: .constant(false),
                      errorText: .constant(""),
                      isMandatory: false)
    }
    
    /// Carrier
    private var carrierField: some View {
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
            EIDInputField(title: "certificate_carrier_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $deviceName,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
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
            hideKeyboard()
        },
                      isMandatory: true)
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
        serialNumber = ""
        certificateName = ""
        selectedAdministrator = .default
        setStatus(nil)
        setDevice(nil)
        clearValidFromDate()
        clearValidUntilDate()
        applyFilter()
    }
    
    private func setupFields() {
        setAdministratorItems()
        statusText = status?.title.localized() ?? "option_all".localized()
        setDevice(deviceId)
        if let date = validFromDateStr.toDate(withFormats: [.iso8601Date]) {
            validFromDate = date
            setValidFromDate()
        } else {
            clearValidFromDate()
        }
        if let date = validUntilDateStr.toDate(withFormats: [.iso8601Date]) {
            validUntilDate = date
            setValidUntilDate()
        } else {
            clearValidUntilDate()
        }
    }
    
    private func setAdministratorItems() {
        administratorItems.removeAll()
        administratorItems.append(PickerViewItem(id: "", name: "option_all".localized()))
        let administratorsToAdd = administrators.map { PickerViewItem(id: $0.id, name: $0.name) }
        administratorItems.append(contentsOf: administratorsToAdd)
    }
    
    private func setStatus(_ newValue: CertificateStatus?) {
        status = newValue
        statusText = status?.title.localized() ?? "option_all".localized()
    }
    
    private func setDevice(_ newValue: String?) {
        deviceId = newValue
        deviceName = devices.first(where: { $0.id == deviceId })?.name ?? "option_all".localized()
    }
    
    private func setValidFromDate() {
        validFromDateStr = validFromDate.normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func clearValidFromDate() {
        validFromDate = .now
        validFromDateStr = ""
    }
    
    private func setValidUntilDate() {
        validUntilDateStr = validUntilDate.normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func clearValidUntilDate() {
        validUntilDate = .now
        validUntilDateStr = ""
    }
}

// MARK: - Preview
#Preview {
    CertificatesFilterView(administrators: .constant([EIDAdministrator]()),
                           devices: .constant([EIDDevice]()),
                           status: .constant(nil),
                           serialNumber: .constant(""),
                           validFromDateStr: .constant(""),
                           validUntilDateStr: .constant(""),
                           deviceId: .constant(nil),
                           certificateName: .constant(""),
                           selectedAdministrator: .constant(EIDAdministrator.default),
                           applyFilter: {},
                           dismiss: {})
}
