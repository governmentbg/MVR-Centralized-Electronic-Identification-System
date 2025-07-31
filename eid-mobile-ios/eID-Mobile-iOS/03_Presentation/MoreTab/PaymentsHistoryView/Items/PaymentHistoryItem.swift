//
//  PaymentHistoryItem.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import SwiftUI

struct PaymentHistoryItem: View {
    // MARK: - Properties
    @State var paymentNumber: String
    @State var createdOn: String
    @State var subject: PaymentHistoryReason
    @State var paymentDate: String
    @State var status: PaymentHistoryStatus
    @State var validUntil: String
    @State var payment: [Payment]
    @State var lastUpdated: String
    
    private let titleMinWidth: CGFloat = 128
    
    // MARK: Body
    var body: some View {
        VStack(spacing: 16) {
            VStack(spacing: 8) {
                paymentNumberView
                createdOnView
                subjectView
                paymentDateView
                validUntilView
                amountView
                currencyView
                lastUpdatedView
            }
            Divider()
            statusView
        }
        .padding()
        .frame(maxWidth: .infinity)
        .background(Color.backgroundWhite)
    }
    
    // MARK: - Child views
    private var paymentNumberView: some View {
        HStack {
            Text("payment_history_payment_number_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(paymentNumber)
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var createdOnView: some View {
        HStack {
            Text("payment_history_created_on_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(createdOn.toDate()?.normalizeDate(outputFormat: .iso8601Date) ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var subjectView: some View {
        HStack {
            Text("payment_history_subject_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(subject.title.localized())
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var paymentDateView: some View {
        HStack {
            Text("payment_history_payment_date_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(paymentDate.toDate()?.normalizeDate(outputFormat: .iso8601DateTime) ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var validUntilView: some View {
        HStack {
            Text("payment_history_valid_until_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(validUntil.toDate()?.normalizeDate(outputFormat: .iso8601DateTime) ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var amountView: some View {
        let paymentPrice = payment.map({ String(format: "%.2f", $0.fee ?? 0) }).joined(separator: " | ")
        return HStack {
            Text("payment_history_amount_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(paymentPrice)
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var currencyView: some View {
        let paymentCurrencty = payment.map({ $0.feeCurrency ?? "" }).joined(separator: " | ")
        return HStack {
            Text("application_payment_view_currency".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(paymentCurrencty)
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var lastUpdatedView: some View {
        HStack {
            Text("payment_history_last_updated_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            Text(lastUpdated.toDate()?.normalizeDate(outputFormat: .iso8601DateTime) ?? "")
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
    
    private var statusView: some View {
        HStack {
            Text("payment_history_status_title".localized())
                .font(.tiny)
                .foregroundStyle(.themePrimaryMedium)
                .frame(minWidth: titleMinWidth, alignment: .leading)
            Spacer()
            HStack {
                Image(status.iconName)
                Text(status.title.localized())
                    .font(.bodyRegular)
                    .lineSpacing(8)
                    .foregroundStyle(status.textColor)
                    .multilineTextAlignment(.trailing)
            }
        }
    }
    
}

#Preview {
    PaymentHistoryItem(paymentNumber: "",
                       createdOn: "",
                       subject: .unknown,
                       paymentDate: "",
                       status: .unknown,
                       validUntil: "",
                       payment: [Payment(fee: 100, feeCurrency: "BGN"),
                                 Payment(fee: 61.36, feeCurrency: "EUR")],
                       lastUpdated: "")
}
