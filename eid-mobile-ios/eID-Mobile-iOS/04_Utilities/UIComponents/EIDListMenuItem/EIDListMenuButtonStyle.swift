//
//  EIDListMenuButtonStyle.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 31.10.23.
//

import SwiftUI


struct ListMenuButtonStyle: ButtonStyle {
    // MARK: - Init
    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .frame(maxWidth: .infinity, minHeight: 80)
            .background(Color.backgroundWhite)
            .clipShape(RoundedRectangle(cornerRadius: 12))
            .addItemShadow()
    }
}
