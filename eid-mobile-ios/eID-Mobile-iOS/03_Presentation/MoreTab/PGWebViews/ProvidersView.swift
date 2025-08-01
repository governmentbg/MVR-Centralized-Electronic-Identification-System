//
//  ProvidersView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 17.09.24.
//

import SwiftUI


struct ProvidersView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .providers)
            .addNavigationBar(title: MoreMenuOption.providers.localizedTitle(),
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
    ProvidersView()
}
