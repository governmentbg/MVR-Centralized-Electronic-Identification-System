//
//  AdministratorsWebView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.09.24.
//

import SwiftUI


struct AdministratorsWebView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .administrators)
            .addNavigationBar(title: MoreMenuOption.administrators.localizedTitle(),
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
    AdministratorsWebView()
}
