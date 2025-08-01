//
//  CentersView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.09.24.
//

import SwiftUI


struct CentersView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .centers)
            .addNavigationBar(title: MoreMenuOption.centers.localizedTitle(),
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
    CentersView()
}
