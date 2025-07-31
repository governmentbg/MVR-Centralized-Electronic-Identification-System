//
//  EmpowermentItemProvider.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.04.25.
//

import SwiftUI

struct EmpowermentItemProvider: View {
    // MARK: - Properties
    @State var providerName: String
    var minWidth: CGFloat = 0
    
    // MARK: - Body
    var body: some View {
        HStack {
            Text("empowerment_provider_title".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: minWidth, alignment: .leading)
            Spacer()
            VStack(alignment: .trailing) {
                Text(providerName)
                    .font(.bodyRegular)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textDefault)
                    .multilineTextAlignment(.trailing)
            }
        }
    }
}

#Preview {
    EmpowermentItemProvider(providerName: "Фирма 2")
}
