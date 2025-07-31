//
//  TexfFieldLimitModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 21.05.24.
//

import SwiftUI


struct TexfFieldLimitModifier: ViewModifier {
    // MARK: - Properties
    @Binding var value: String
    var length: Int
    
    // MARK: - Body
    func body(content: Content) -> some View {
        Group {
            content
                .onReceive(value.publisher.collect()) {
                    value = String($0.prefix(length))
                }
        }
    }
}
