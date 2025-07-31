//
//  EIDTabItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 10.10.23.
//

import SwiftUI


struct EIDTabItem: View {
    // MARK: - Properties
    @State var iconName: String
    @State var title: String
    
    // MARK: - Body
    var body: some View {
        HStack {
            Image(iconName)
                .resizable()
            Text(title.localized())
                .font(.bodySmall)
        }
    }
}
