//
//  EIDInfoView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.10.24.
//

import SwiftUI


struct EIDInfoView: View {
    // MARK: - Properties
    @Binding var title: String
    @Binding var description: String
    
    // MARK: - Body
    var body: some View {
        VStack {
            Text(title)
                .font(.headline)
                .foregroundColor(.textDark)
                .padding()
            GradientDivider()
            ScrollView {
                MarkdownRenderer(description)
                    .foregroundColor(.textDark)
                    .padding()
                Spacer()
            }
        }
        .padding()
        .background(Color.backgroundWhite)
    }
}
