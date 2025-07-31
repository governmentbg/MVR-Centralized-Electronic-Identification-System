//
//  EIDBackButton.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 12.07.24.
//

import SwiftUI


struct EIDBackButton: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        Button(action: {
            presentationMode.wrappedValue.dismiss()
        }, label: {
            Image("icon_back_dark")
                .resizable()
                .renderingMode(.template)
                .foregroundColor(.buttonDefault)
                .frame(width: 24.0, height: 24.0)
        })
    }
}
