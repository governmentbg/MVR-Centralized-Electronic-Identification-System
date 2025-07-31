//
//  TermsAndConditionsView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.06.24.
//

import SwiftUI


struct TermsAndConditionsView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .termsAndConditions)
            .addNavigationBar(title: MoreMenuOption.termsAndConditions.localizedTitle(),
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
    TermsAndConditionsView()
}
