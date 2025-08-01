//
//  EmpowermentItemNumber.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 16.04.25.
//

import SwiftUI

struct EmpowermentItemNumber: View {
    // MARK: - Properties
    @State var empowermentNumber: String
    
    // MARK: - Body
    var body: some View {
        VStack(alignment: .leading) {
            Text("empowerments_number_title".localized())
                .font(.heading4)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
                .padding(.bottom, 4)
            Text(empowermentNumber)
                .font(.heading4)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
        }
    }
}

#Preview {
    EmpowermentItemNumber(empowermentNumber: "lsdkjflksdjlkjdsf")
}
