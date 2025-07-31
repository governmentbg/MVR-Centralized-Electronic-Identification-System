//
//  EmpowermentMenuOptionItem.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.11.23.
//

import SwiftUI

enum MenuOptionItemAlignment {
    case leading
    case center
    case trailing
}


struct MenuOptionItem: View {
    // MARK: - Properties
    @State var icon: String
    @State var text: String
    @State var textColor: Color
    @State var iconColor: Color?
    @State var padding: CGFloat?
    @State var alignment: MenuOptionItemAlignment = .center
    
    // MARK: - Body
    var body: some View {
        let iconImage = UIImage(named: icon) ?? UIImage(systemName: icon)
        return HStack {
            if alignment == .trailing {
                Spacer()
            }
            Image(uiImage: iconImage ?? UIImage())
                .renderingMode(.template)
                .resizable()
                .frame(width: 25, height: 25)
                .foregroundColor(iconColor ?? .buttonDefault)
            if alignment == .center {
                Spacer()
            }
            Text(text)
                .font(.bodyRegular)
                .foregroundStyle(textColor)
            if alignment == .leading {
                Spacer()
            }
        }
        .padding(padding ?? 12)
    }
}
