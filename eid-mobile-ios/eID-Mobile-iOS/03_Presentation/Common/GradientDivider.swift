//
//  GradientDivider.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.05.24.
//

import SwiftUI


struct GradientDivider: View {
    // MARK: - Body
    var body: some View {
        Divider()
            .frame(height: 1)
            .frame(maxWidth: .infinity)
            .overlay(LinearGradient(colors: Color.gradientColors,
                                    startPoint: .leading,
                                    endPoint: .trailing))
    }
}

#Preview {
    GradientDivider()
}
