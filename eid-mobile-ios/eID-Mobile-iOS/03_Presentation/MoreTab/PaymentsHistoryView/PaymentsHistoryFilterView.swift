//
//  PaymentsHistoryFilterView.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import SwiftUI

struct PaymentsHistoryFilterView: View {
    // MARK: - Properties
    /// Filter data to send
    @Binding var paymentNumber: String
    @Binding var status: PaymentHistoryStatus?
    @Binding var createdOnDateStr: String
    @Binding var reason: PaymentHistoryReason?
    @Binding var amount: PaymentHistoryAmount?
    @Binding var paymentDateDateStr: String
    @Binding var validUntilDateStr: String
    /// Fields
    @State private var statusText: String = "option_all".localized()
    @State private var reasonText: String = "option_all".localized()
    @State private var amountText: String = "option_all".localized()
    /// Callbacks
    var applyFilter: () -> Void
    var dismiss: () -> Void
    /// Private
    @State private var createdOnDate: Date = .now
    @State private var paymentDateDate: Date = .now
    @State private var validUntilDate: Date = .now
    @State private var showCreatedOnDatePicker: Bool = false
    @State private var showPaymentDateDatePicker: Bool = false
    @State private var showValidUntilDatePicker: Bool = false
    @FocusState private var focusState: Bool
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            titleView
            Divider()
            ScrollView {
                VStack(spacing: 24) {
                    paymentNumberField
                    statusField
                    createdOnDateField
                    subjectField
                    amountField
                    paymentDateDateField
                    validUntilDateField
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
    
    /// Serial number
    private var paymentNumberField: some View {
        EIDInputField(title: "payment_history_payment_number_title".localized(),
                      text: $paymentNumber,
                      showError: .constant(false),
                      errorText: .constant(""))
    }
    
    /// Status
    private var statusField: some View {
        Menu(content: {
            Button(action: {
                setStatus(nil)
            }, label: {
                Text("option_all".localized())
            })
            ForEach(PaymentHistoryStatus.allCases, id: \.self) { status in
                Button(action: {
                    setStatus(status)
                }, label: {
                    Text(status.title.localized())
                })
            }
        }, label: {
            EIDInputField(title: "payment_history_status_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $statusText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    /// Created on date
    private var createdOnDateField: some View {
        EIDInputField(title: "payment_history_created_on_title".localized(),
                      text: $createdOnDateStr,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showCreatedOnDatePicker = true
        })
        .showDatePicker(showPicker: $showCreatedOnDatePicker,
                        selectedDate: $createdOnDate,
                        rangeThrough: ...Date(),
                        title: "payment_history_created_on_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setCreatedOnDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearCreatedOnDate() })
    }
    
    /// Subject
    private var subjectField: some View {
        Menu(content: {
            Button(action: {
                setStatus(nil)
            }, label: {
                Text("option_all".localized())
            })
            ForEach(PaymentHistoryReason.allCases, id: \.self) { reason in
                Button(action: {
                    setReason(reason)
                }, label: {
                    Text(reason.title.localized())
                })
            }
        }, label: {
            EIDInputField(title: "payment_history_subject_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $reasonText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    /// Subject
    private var amountField: some View {
        Menu(content: {
            Button(action: {
                setAmount(nil)
            }, label: {
                Text("option_all".localized())
            })
            ForEach([PaymentHistoryAmount.below(value: 25),PaymentHistoryAmount.between(value: 25...50),PaymentHistoryAmount.above(value: 50)], id: \.self) { amount in
                Button(action: {
                    setAmount(amount)
                }, label: {
                    Text(amount.title)
                })
            }
        }, label: {
            EIDInputField(title: "payment_history_amount_title".localized(),
                          hint: "hint_please_select".localized(),
                          text: $amountText,
                          showError: .constant(false),
                          errorText: .constant(""),
                          rightIcon: .arrowDown)
        })
        .preferredColorScheme(.light)
    }
    
    /// Payment date
    private var paymentDateDateField: some View {
        EIDInputField(title: "payment_history_payment_date_title".localized(),
                      text: $paymentDateDateStr,
                      showError: .constant(false),
                      errorText: .constant(""),
                      rightIcon: .calendar,
                      isPicker: true,
                      tapAction: {
            focusState = false
            showPaymentDateDatePicker = true
        })
        .showDatePicker(showPicker: $showPaymentDateDatePicker,
                        selectedDate: $paymentDateDate,
                        rangeThrough: ...Date(),
                        title: "payment_history_payment_date_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setPaymentDateDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearPaymentDateDate() })
    }
    
    /// Valid until date
    private var validUntilDateField: some View {
        EIDInputField(title: "payment_history_valid_until_title".localized(),
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
                        title: "payment_history_valid_until_title".localized(),
                        submitButtonTitle: "btn_select".localized(),
                        onSubmit: { setValidUntilDate() },
                        canClear: true,
                        clearButtonTitle: "btn_clear".localized(),
                        onClear: { clearValidUntilDate() })
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
        paymentNumber = ""
        setStatus(nil)
        setReason(nil)
        setAmount(nil)
        clearCreatedOnDate()
        clearPaymentDateDate()
        clearValidUntilDate()
        applyFilter()
    }
    
    private func setupFields() {
        statusText = status?.title.localized() ?? "option_all".localized()
        reasonText = reason?.title.localized() ?? "option_all".localized()
        amountText = amount?.title ?? "option_all".localized()
        if let date = createdOnDateStr.toDate(withFormats: [.iso8601Date]) {
            createdOnDate = date
            setCreatedOnDate()
        } else {
            clearCreatedOnDate()
        }
        if let date = paymentDateDateStr.toDate(withFormats: [.iso8601Date]) {
            paymentDateDate = date
            setPaymentDateDate()
        } else {
            clearPaymentDateDate()
        }
        if let date = validUntilDateStr.toDate(withFormats: [.iso8601Date]) {
            validUntilDate = date
            setValidUntilDate()
        } else {
            clearValidUntilDate()
        }
    }
    
    private func setStatus(_ newValue: PaymentHistoryStatus?) {
        status = newValue
        statusText = status?.title.localized() ?? "option_all".localized()
    }
    
    private func setReason(_ newValue: PaymentHistoryReason?) {
        reason = newValue
        reasonText = status?.title.localized() ?? "option_all".localized()
    }
    
    private func setAmount(_ newValue: PaymentHistoryAmount?) {
        amount = newValue
        amountText = amount?.title ?? "option_all".localized()
    }
    
    private func setCreatedOnDate() {
        createdOnDateStr = createdOnDate.normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func clearCreatedOnDate() {
        createdOnDate = .now
        createdOnDateStr = ""
    }
    
    private func setPaymentDateDate() {
        paymentDateDateStr = paymentDateDate.normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func clearPaymentDateDate() {
        paymentDateDate = .now
        paymentDateDateStr = ""
    }
    
    private func setValidUntilDate() {
        validUntilDateStr = validUntilDate.normalizeDate(outputFormat: .iso8601Date)
    }
    
    private func clearValidUntilDate() {
        validUntilDate = .now
        validUntilDateStr = ""
    }
}

#Preview {
    PaymentsHistoryFilterView(
        paymentNumber: .constant(""),
        status: .constant(nil),
        createdOnDateStr: .constant(""),
        reason: .constant(nil),
        amount: .constant(nil),
        paymentDateDateStr: .constant(""),
        validUntilDateStr: .constant(""),
        applyFilter: {},
        dismiss: {}
    )
}
