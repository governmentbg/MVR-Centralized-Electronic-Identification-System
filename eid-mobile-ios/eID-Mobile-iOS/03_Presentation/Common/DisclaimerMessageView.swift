//
//  DisclaimerMessageView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 20.05.24.
//

import SwiftUI


struct DisclaimerMessageView: View {
    // MARK: - Properties
    var title: String
    
    // MARK: - Body
    var body: some View {
        HStack {
            Image("icon_info")
                .renderingMode(.template)
                .resizable()
                .frame(width: 20.0, height: 20.0)
                .foregroundColor(Color.textActive)
                .padding([.trailing], 8)
                .padding([.leading], 16)
            Text(title)
                .foregroundStyle(Color.textActive)
                .font(.tiny)
                .padding([.trailing], 16)
            Spacer()
        }
        .padding([.top, .bottom], 16)
        .frame(maxWidth: .infinity)
        .background(Color.themePrimaryLight)
        .cornerRadius(8)
    }
}

#Preview {
    DisclaimerMessageView(title: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque pharetra sapien id finibus tempus")
}
