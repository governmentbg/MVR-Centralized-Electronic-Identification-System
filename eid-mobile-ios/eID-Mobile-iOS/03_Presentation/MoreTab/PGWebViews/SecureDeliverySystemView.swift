//
//  SecureDeliverySystemView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 17.09.24.
//

import SwiftUI


struct SecureDeliverySystemView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .secureDeliverySystem)
            .addNavigationBar(title: MoreMenuOption.secureDeliverySystem.localizedTitle(),
                              content: {
                ToolbarItem(placement: .topBarLeading) {
                    Button(action: {
                        presentationMode.wrappedValue.dismiss()
                    }, label: {
                        Image("icon_arrow_left")
                    })
                }
            })
    }
}

#Preview {
    SecureDeliverySystemView()
}
