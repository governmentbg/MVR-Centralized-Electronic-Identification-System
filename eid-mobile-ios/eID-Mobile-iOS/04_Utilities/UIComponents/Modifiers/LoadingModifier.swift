//
//  LoadingModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 10.10.23.
//

import SwiftUI


struct LoadingModifier: ViewModifier {
    // MARK: - Properties
    @Binding var isLoading: Bool
    @State var blurRadius: CGFloat = 2
    var details: String?
    
    // MARK: - Body
    func body(content: Content) -> some View {
        Group {
            content
                .blur(radius: isLoading ? blurRadius : 0)
                .animation(.easeIn, value: 1)
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .allowsHitTesting(!isLoading)
        .overlay {
            if isLoading {
                VStack {
                    if details != nil {
                        Text(details ?? "")
                            .font(.heading4)
                            .lineSpacing(8)
                            .foregroundStyle(Color.textDefault)
                            .multilineTextAlignment(.center)
                            .padding()
                    }
                    ProgressView()
                        .progressViewStyle(.circular)
                        .controlSize(.large)
                        .tint(.textActive)
                        .background(Color.clear)
                }
            }
        }
    }
}
