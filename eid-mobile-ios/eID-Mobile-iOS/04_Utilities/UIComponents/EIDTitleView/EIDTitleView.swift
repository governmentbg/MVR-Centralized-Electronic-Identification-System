//
//  EIDTitleView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 8.07.24.
//

import SwiftUI


struct EIDTitleView: View {
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            Image("img_logo")
                .resizable()
                .aspectRatio(1/1, contentMode: .fit)
                .frame(width: 100)
                .clipped()
                .padding([.bottom], 16)
            Text("bgeid_title".localized())
                .font(.heading1)
                .foregroundStyle(Color.themeSecondaryDark)
                .padding()
                .frame(maxWidth: .infinity)
            Text("country_title".localized())
                .font(.bodySmall)
                .foregroundStyle(Color.textDark)
                .padding([.bottom], 8)
                .frame(maxWidth: .infinity)
            Text("mvr_title".localized())
                .font(.bodySmallBold)
                .foregroundStyle(Color.textDark)
                .frame(maxWidth: .infinity)
        }
    }
}

#Preview {
    EIDTitleView()
}
