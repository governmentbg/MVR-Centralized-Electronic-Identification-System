//
//  ScrollerToTop.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 21.06.24.
//

import SwiftUI

struct ScrollerToTopViewModifier: ViewModifier {
    // MARK: - Properties
    @Binding var scrollOnChange: Bool
    
    // MARK: - Body
    func body(content: Content) -> some View {
        ScrollViewReader { reader in
            content
                .onChange(of: scrollOnChange) { _ in
                    reader.scrollTo("topScrollPoint", anchor: .top)
                    scrollOnChange = false
                }
        }
    }
}

extension View {
    func scrollToTopHandler(scrollOnChange: Binding<Bool>) -> some View {
        self.modifier(ScrollerToTopViewModifier(scrollOnChange: scrollOnChange))
    }
}
