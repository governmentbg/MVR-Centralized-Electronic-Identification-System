//
//  ContactsView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.06.24.
//

import SwiftUI


struct ContactsView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .contacts)
            .addNavigationBar(title: MoreMenuOption.contactUs.localizedTitle(),
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
    ContactsView()
}
