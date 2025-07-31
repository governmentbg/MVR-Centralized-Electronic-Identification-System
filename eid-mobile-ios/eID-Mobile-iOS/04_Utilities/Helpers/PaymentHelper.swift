//
//  PaymentHelper.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.12.24.
//

import SwiftUI


final class PaymentHelper: ObservableObject {
    // MARK: - Properties
    static let shared = PaymentHelper()
    
    // MARK: - Init
    static func openPaymentView(paymentAccessCode: String) {
        guard var paymenAddress = AppConfiguration.get(keychainKey: ServiceDomain.PAYMENT.baseUrl) as? String,
        let paymentCodeQuery = AppConfiguration.get(keychainKey: .paymentCode) as? String else {
            return
        }
        
        if paymentAccessCode.isEmpty == false {
            paymenAddress += paymentCodeQuery + paymentAccessCode
        }
        
        guard let url = URL(string: paymenAddress) else {
            return
        }
        
        UIApplication.shared.open(url)
    }
}
