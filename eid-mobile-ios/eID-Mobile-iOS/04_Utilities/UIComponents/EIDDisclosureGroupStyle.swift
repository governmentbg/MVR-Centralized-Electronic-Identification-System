//
//  EIDDisclosureGroupStyle.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.10.23.
//

import SwiftUI


struct EIDDisclosureGroupStyle: DisclosureGroupStyle {
    // MARK: - Body
    func makeBody(configuration: Configuration) -> some View {
        VStack(spacing: 0) {
            Button {
                withAnimation {
                    configuration.isExpanded.toggle()
                }
            } label: {
                HStack(alignment: .center) {
                    configuration.label
                    Spacer()
                    Image(configuration.isExpanded ? "icon_arrow_down_dark" : "icon_arrow_right_dark")
                        .animation(nil, value: configuration.isExpanded)
                }
                .contentShape(Rectangle())
                .border(width: 1,
                        edges: [.bottom],
                        color: .themePrimaryLight)
            }
            .buttonStyle(.plain)
            if configuration.isExpanded {
                configuration.content
            }
        }
    }
}
