//
//  OnlineHelpSystemWebView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.10.24.
//

import SwiftUI

// MARK: - Help system link by chosen config environment
struct OnlineHelpSystemLinkByEnvironment {
    static var link: String {
        switch AppConfiguration.currentEnvironment() {
        case .mvrDev, .mvrTest, .mvrStage:
            return "http://10.246.194.26/otrs/customer.pl"
        default:
            return "http://10.34.32.11/otrs/customer.pl?"
        }
    }
}


struct OnlineHelpSystemWebView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        EIDWebView(type: .onlineHelpSystem)
            .addNavigationBar(title: MoreMenuOption.onlineHelpSystem.localizedTitle(),
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
    OnlineHelpSystemWebView()
}
