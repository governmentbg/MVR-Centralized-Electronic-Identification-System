//
//  FAQView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.06.24.
//

import SwiftUI


struct FAQView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .faq)
            .addNavigationBar(title: MoreMenuOption.faq.localizedTitle(),
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
    FAQView()
}
