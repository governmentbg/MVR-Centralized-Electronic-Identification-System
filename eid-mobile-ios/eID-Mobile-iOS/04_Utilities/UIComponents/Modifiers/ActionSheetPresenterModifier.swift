//
//  ActionSheetPresenterModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.08.24.
//

import SwiftUI


struct ActionSheetPresenterModifier: ViewModifier {
    // MARK: - Properties
    @Binding var showOptions: Bool
    var title: String
    var options: [String]
    var onOptionPicked: ((String) -> Void)? = nil
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .confirmationDialog(title, isPresented: $showOptions, titleVisibility: .visible) {
                ForEach(options, id: \.self) { option in
                    Button(option) {
                        onOptionPicked?(option)
                    }
                }
            }
    }
}
