//
//  AlertPresenterModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import SwiftUI


struct AlertPresenterModifier: ViewModifier {
    // MARK: - Properties
    @Binding var showAlert: Bool
    @Binding var alertText: String
    var onDismiss: (() -> Void)? = nil
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .alert("info_title".localized(),
                   isPresented: $showAlert) {
                Button("btn_ok".localized(), role: .cancel) {
                    onDismiss?()
                }
            } message: {
                Text(alertText)
                    .font(.bodyRegular)
            }
    }
}
