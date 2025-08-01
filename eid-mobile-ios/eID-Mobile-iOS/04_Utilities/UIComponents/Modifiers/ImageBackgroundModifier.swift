//
//  ImageBackgroundModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.07.24.
//

import SwiftUI


struct ImageBackgroundModifier: ViewModifier {
    // MARK: - Properties
    var image: String?
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .background(
                ZStack {
                    Image(image ?? "img_main_login_background")
                        .resizable()
                }
                    .ignoresSafeArea()
            )
    }
}

extension View {
    func setBackground(image: String? = nil) -> some View {
        self.modifier(ImageBackgroundModifier(image: image))
    }
}
