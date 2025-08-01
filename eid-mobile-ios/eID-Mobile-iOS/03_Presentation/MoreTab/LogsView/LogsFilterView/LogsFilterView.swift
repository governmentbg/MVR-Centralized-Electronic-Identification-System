//
//  LogsFilterView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.10.24.
//

import SwiftUI


struct LogsFilterView: View {
    // MARK: - Properties
    @Binding var logsDescriptions: [LocalisedLog]
    /// Filter data to send
    @Binding var startDateStr: String
    @Binding var endDateStr: String
    @Binding var type: [String]
    /// Callbacks
    var applyFilter: () -> Void
    var dismiss: () -> Void
    /// Private
    @State private var startDate: Date = .now
    @State private var endDate: Date = .now
    @State private var showStartDatePicker: Bool = false
    @State private var showEndDatePicker: Bool = false
    @State private var showTypeList: Bool = false
    @State private var logTypeItems: [MultiPickerViewItem] = []
    @State private var allTypesSelected: Bool = false
    @State private var typeStr = ""
    @FocusState private var focusState: Bool
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            Divider()
            ScrollView {
                VStack(spacing: 24) {
                    typeField
                    startDateField
                    endDateField
                }
                .padding()
            }
            Divider()
            filterButtons
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.backgroundWhite)
        .onAppear {
            typeStr = type.joined(separator: ", ")
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
    
    /// Type
    private var typeField: some View {
        EIDInputField(title: "logs_type_title".localized(),
                      hint: "hint_please_select".localized(),
                      text: $typeStr,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .arrowDown,
                      isPicker: true,
                      tapAction: {
            showTypeList.toggle()
        })
        .sheet(isPresented: $showTypeList,
               content: {
            EIDSearchableMultiPickerView(title: "logs_type_title".localized(),
                                         items: $logTypeItems,
                                         allSelected: $allTypesSelected,
                                         showAllSelectedToggle: true,
                                         onSelectionChange: { typeChanged() })
        })
    }
    
    /// Start date
    private var startDateField: some View {
        EIDInputField(title: "logs_start_date_title".localized(),
                      text: $startDateStr,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showStartDatePicker = true
        })
        .showDatePicker(showPicker: $showStartDatePicker,
                        selectedDate: $startDate,
                        rangeThrough: ...Date(),
                        title: "logs_start_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setStartDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearStartDate() })
    }
    
    /// End date
    private var endDateField: some View {
        EIDInputField(title: "logs_еnd_date_title".localized(),
                      text: $endDateStr,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showEndDatePicker = true
        })
        .showDatePicker(showPicker: $showEndDatePicker,
                        selectedDate: $endDate,
                        rangeThrough: ...Date(),
                        title: "logs_еnd_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setEndDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearEndDate() })
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
        type = []
        typeStr = ""
        clearStartDate()
        clearEndDate()
        applyFilter()
    }
    
    private func setupFields() {
        setTypeItems()
        allTypesSelected = logTypeItems.allSatisfy { $0.selected }
        if let date = startDateStr.toDate(withFormats: [.iso8601Date]) {
            startDate = date
            setStartDate()
        } else {
            clearStartDate()
        }
        if let date = endDateStr.toDate(withFormats: [.iso8601Date]) {
            endDate = date
            setEndDate()
        } else {
            clearEndDate()
        }
    }
    
    private func setStartDate() {
        startDateStr = startDate.toLocalTime().normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func clearStartDate() {
        startDate = .now
        startDateStr = ""
    }
    
    private func setEndDate() {
        endDateStr = endDate.toLocalTime().normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func clearEndDate() {
        endDate = .now
        endDateStr = ""
    }
    
    private func setTypeItems() {
        logTypeItems = logsDescriptions.map { MultiPickerViewItem(id: $0.key,
                                                                  name: $0.localisedDescription,
                                                                  selected: type.contains($0.localisedDescription) ) }
        typeChanged()
    }
    
    private func typeChanged() {
        let filteredLogTypeItems = logTypeItems.filter{ $0.selected }
        type = filteredLogTypeItems.map { $0.name }
        typeStr = type.joined(separator: ", ")
    }
}
