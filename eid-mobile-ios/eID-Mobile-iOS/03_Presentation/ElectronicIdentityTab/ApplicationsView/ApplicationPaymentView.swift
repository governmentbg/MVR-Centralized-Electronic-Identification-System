//
//  ApplicationPaymentView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.12.24.
//

import SwiftUI

struct ApplicationPaymentView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @Binding var path: [String]
    var payment: [Payment]?
    
    // MARK: - Body
    var body: some View {
        VStack {
            titleView
            detailView
            itemsTitleView
            itemsDetailsView
            receiptView
            
            Spacer()
            Spacer()
            
            payButton
            closeButton
            
            Spacer()
        }
        .addNavigationBar(title: "application_screen_title".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
    }
    
    // MARK: - Child views
    private var titleView: some View {
        Text("application_payment_view_title".localized())
            .font(.heading2)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .frame(maxWidth: .infinity, alignment: .leading)
            .padding()
    }
    
    private var detailView: some View {
        Text("application_payment_view_details".localized())
            .font(.bodyRegular)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .frame(maxWidth: .infinity, alignment: .leading)
            .padding()
    }
    
    private var itemsTitleView: some View {
        Text("application_payment_view_items_title".localized())
            .font(.bodyRegular)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .frame(maxWidth: .infinity, alignment: .leading)
            .padding([.leading, .trailing], 16)
            .padding(.top, 8)
            .padding(.bottom, 2)
    }
    
    private var itemsDetailsView: some View {
        Text("\u{2022} " + "personal_identification_document".localized())
            .font(.bodyRegular)
            .lineSpacing(8)
            .foregroundStyle(Color.textDark)
            .frame(maxWidth: .infinity, alignment: .leading)
            .padding(.leading, 24)
            .padding(.trailing, 16)
            .padding(.top, 6)
            .padding(.bottom, 16)
    }
    
    private var receiptView: some View {
        VStack {
            Text("application_payment_view_payment_details_title".localized())
                .font(.bodyBold)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .frame(maxWidth: .infinity, alignment: .leading)
                .padding()
            
            let paymentPrice = payment?.map({ String(format: "%.2f", $0.fee ?? 0) }).joined(separator: " | ")
            receiptTableItemView(key: "application_payment_view_price".localized(),
                                 value: paymentPrice ?? "")
            
            receiptTableItemView(key: "application_payment_view_device_price".localized(),
                                 value: "0")
            
            let paymentCurrencty = payment?.map({ $0.feeCurrency ?? "" }).joined(separator: " | ")
            receiptTableItemView(key: "application_payment_view_currency".localized(),
                                 value: paymentCurrencty ?? "")
        }
    }
    
    private func receiptTableItemView(key: String, value: String) -> some View {
        HStack {
            Text(key)
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .padding()
            
            Spacer()
            
            Text(value)
                .font(.bodyBold)
                .lineSpacing(8)
                .foregroundStyle(Color.textActive)
                .padding()
        }
        .frame(maxWidth: .infinity)
    }
    
    
    // Buttons
    private var payButton: some View {
        Button(action: {
            PaymentHelper.openPaymentView(paymentAccessCode: payment?.first?.accessCode ?? "")
            path.removeAll()
        }, label: {
            Text("btn_payment".localized())
        })
        .buttonStyle(EIDButton(buttonType: .filled))
        .padding([.leading, .trailing], 16)
        .padding(.bottom, 8)
    }
    
    private var closeButton: some View {
        Button(action: {
            presentationMode.wrappedValue.dismiss()
        }, label: {
            Text("btn_close".localized())
        })
        .buttonStyle(EIDButton(buttonType: .outline))
        .padding([.leading, .trailing], 16)
    }
}

#Preview {
    ApplicationPaymentView(path: .constant([""]),
                           payment:
                            [Payment(fee: 100,
                                     feeCurrency: "BGN"),
                             Payment(fee: 61.36,
                                     feeCurrency: "EUR")]
    )
}
