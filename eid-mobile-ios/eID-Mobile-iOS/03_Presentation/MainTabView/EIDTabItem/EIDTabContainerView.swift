//
//  EIDTabContainerView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 10.10.23.
//

import SwiftUI


struct EIDTabContainerView: View {
    // MARK: - Properties
    @State var contentView: AnyView
    
    // MARK: - Body
    var body: some View {
        ZStack {
            Color.themeSecondaryDark
                .ignoresSafeArea()
            VStack {
                contentView
                    .frame(maxHeight: .infinity)
                Rectangle()
                    .fill(Color.clear)
                    .frame(height: 0)
            }
        }
    }
}
