//
//  DetailsViewRow.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 15.11.23.
//

import SwiftUI


struct DetailsViewRow: View {
    // MARK: - Properties
    @State var title: String
    @State var specialContent: AnyView? = nil
    var action: Action? = nil
    @State var values: [String]
    @State var valuesColor: Color = .textDefault
    @State var specialContentOnBottom: Bool = false
    
    // MARK: - Body
    var body: some View {
        VStack(alignment: .leading, spacing: 4) {
            Text(title)
                .font(.tiny)
                .lineSpacing(4)
                .foregroundStyle(Color.themePrimaryMedium)
                .multilineTextAlignment(.leading)
            if !specialContentOnBottom,
               specialContent != nil {
                specialContent
            }
            
            HStack(alignment: .center, spacing: 8) {
                VStack(alignment:.leading, spacing: 4) {
                    ForEach(values, id: \.self) { value in
                        Text(value)
                            .font(.bodyRegular)
                            .lineSpacing(8)
                            .foregroundStyle(valuesColor)
                            .multilineTextAlignment(.leading)
                    }
                }
                
                if let action {
                    Spacer()
                    Button(action: action.action, label: {
                        Image(action.icon)
                            .renderingMode(.template)
                            .foregroundColor(action.color)
                    })
                }
            }
            
            if specialContentOnBottom,
               specialContent != nil {
                specialContent
            }
        }
        .frame(maxWidth: .infinity, alignment: .leading)
    }
}

#Preview {
    DetailsViewRow(title: "Test", specialContent: nil, action: nil, values: ["Test", "Test" , "Test" ,"Test" ,"Test", "Test"], valuesColor: .textDefault, specialContentOnBottom: false)
}
