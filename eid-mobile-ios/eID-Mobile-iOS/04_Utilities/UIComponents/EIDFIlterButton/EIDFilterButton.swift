//
//  EIDFilterButton.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.09.24.
//

import SwiftUI


struct EIDFilterButton: View {
    // MARK: - Properties
    @Binding var isFilterApplied: Bool
    var action: () -> Void
    
    // MARK: - Body
    var body: some View {
        Button(action: {
            action()
        }, label: {
            Image("icon_filter")
        })
        .padding(4)
        .background(isFilterApplied ? Color.buttonDanger : Color.clear,
                    in: .circle)
    }
}
