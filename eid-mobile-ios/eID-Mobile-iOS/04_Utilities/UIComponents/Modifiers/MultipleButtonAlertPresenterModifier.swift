//
//  MultipleButtonAlertPresenterModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.09.24.
//

import SwiftUI


struct MultipleButtonAlertPresenterModifier: ViewModifier {
    // MARK: - Properties
    @Binding var showAlert: Bool
    @Binding var alertText: String
    var buttons: [AlertButton]
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .alert(alertText, isPresented: $showAlert, actions: {
                ForEach(buttons, id: \.self) { button in
                    Button(button.title.localized(), role: nil, action: {
                        button.action?()
                    })
                }
            })
    }
}


struct AlertButton: Hashable {
    var title: String
    var action: (() -> ())?
    
    func hash(into hasher: inout Hasher) {
        return hasher.combine(title)
    }
    
    static func == (lhs: AlertButton, rhs: AlertButton) -> Bool {
        return lhs.title == rhs.title
    }
}
