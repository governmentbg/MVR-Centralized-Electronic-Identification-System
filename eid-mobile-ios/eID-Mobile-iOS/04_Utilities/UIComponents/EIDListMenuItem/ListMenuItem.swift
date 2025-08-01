//
//  ListMenuItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 31.10.23.
//

import SwiftUI

struct ListMenuItem: View {
    // MARK: - Properties
    var imageName: String
    var title: String
    var subtitle: String
    
    // MARK: - Body
    var body: some View {
        HStack {
            HStack(spacing: 16) {
                Image(imageName)
                VStack(alignment: .leading, spacing: 8) {
                    Text(title)
                        .font(.heading4)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textActive)
                    Text(subtitle)
                        .font(.bodySmall)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textLight)
                }
            }
            Spacer()
            Image("icon_forward_gold")
        }
        .frame(maxWidth: .infinity)
        .padding()
        
    }
}


// MARK: - Preview
#Preview {
    ListMenuItem(imageName: "icon_empowerments_from_me",
                 title: "Title",
                 subtitle: "Subtitle")
}
