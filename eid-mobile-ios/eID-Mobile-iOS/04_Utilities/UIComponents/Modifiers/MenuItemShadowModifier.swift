//
//  MenuItemShadowModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 21.11.23.
//

import SwiftUI


struct MenuItemShadowModifier: ViewModifier {
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .shadow(color: Color.black.opacity(0.04), radius: 5, y: 10)
            .shadow(color: Color.themePrimaryMedium.opacity(0.1), radius: 10)
    }
}
