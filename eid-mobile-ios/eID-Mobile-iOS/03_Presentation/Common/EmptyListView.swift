//
//  EmptyListView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 22.05.24.
//

import SwiftUI


struct EmptyListView: View {
    // MARK: - Body
    var body: some View {
        VStack(spacing: 16) {
            Image("icon_empty_list")
            Text("empty_list_title".localized())
                .font(.bodyRegular)
                .lineSpacing(8)
                .foregroundStyle(Color.textLight)
        }
        .padding(24)
    }
}

#Preview {
    EmptyListView()
}
