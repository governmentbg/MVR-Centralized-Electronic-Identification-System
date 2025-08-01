//
//  GlowModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 28.02.24.
//

import SwiftUI


struct GlowModifier: ViewModifier {
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .shadow(color: Color.black.opacity(0.04), radius: 10, y: 10)
            .shadow(color: Color.buttonGlowBlue.opacity(0.4), radius: 10)
    }
}
