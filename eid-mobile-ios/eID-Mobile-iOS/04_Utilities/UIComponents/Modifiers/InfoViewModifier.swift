//
//  InfoViewModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.10.24.
//

import SwiftUI


struct InfoViewModifier: ViewModifier {
    // MARK: - Properties
    @Binding var showInfo: Bool
    @Binding var title: String
    @Binding var description: String
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .sheet(isPresented: $showInfo,
                   content: {
                EIDInfoView(title: $title,
                            description: $description)
            })
    }
}
