//
//  NoInternetView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 23.05.24.
//

import SwiftUI


struct NoInternetView: View {
    // MARK: - Body
    var body: some View {
        VStack {
            Text("error_no_internet".localized())
                .font(.heading4)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .padding()
                .frame(maxWidth: .infinity)
                .background(.ultraThinMaterial)
            Spacer()
        }
    }
}

#Preview {
    NoInternetView()
}
