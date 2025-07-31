//
//  EmpowermentItemCreatedOn.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.04.25.
//

import SwiftUI

struct EmpowermentItemCreatedOn: View {
    // MARK: - Properties
    @State var createdOn: String
    var minWidth: CGFloat = 0
    
    // MARK: - Body
    var body: some View {
        HStack {
            Text("empowerment_created_on".localized())
                .font(.tiny)
                .foregroundStyle(Color.themePrimaryMedium)
                .frame(minWidth: minWidth, alignment: .leading)
            Spacer()
            Text(createdOn)
                .font(.bodyRegular)
                .lineSpacing(8)
                .lineLimit(2)
                .foregroundStyle(Color.textDefault)
                .multilineTextAlignment(.trailing)
        }
    }
}

#Preview {
    EmpowermentItemCreatedOn(createdOn: "", minWidth: 120)
}
