//
//  LanguageMenu.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.09.24.
//

import SwiftUI


struct LanguageMenu: ToolbarContent {
    // MARK: - Properties
    @Binding var showLanguageAlert: Bool
    private var languageForMenu: Language {
        return LanguageManager.preferredLanguage == .bg ? .en : .bg
    }
    
    // MARK: - Body
    var body: some ToolbarContent {
        ToolbarItem(placement: .topBarTrailing, content: {
            Button(action: {
                showLanguageAlert.toggle()
            }, label: {
                HStack() {
                    Image(languageForMenu.icon)
                        .resizable()
                        .frame(width: 24.0, height: 24.0)
                        .padding([.trailing], 8)
                    Spacer()
                    Text(languageForMenu.description.localized().capitalized)
                        .textCase(.uppercase)
                        .font(.bodySmall)
                        .foregroundColor(.buttonDefault)
                        .frame(maxWidth: .infinity, alignment: .leading)
                }
                .padding(16)
            })
        })
    }
}
