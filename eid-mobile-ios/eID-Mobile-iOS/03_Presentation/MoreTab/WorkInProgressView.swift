//
//  WorkInProgressView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 27.10.23.
//

import SwiftUI


struct WorkInProgressView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        VStack {
            Text("TBD")
                .font(.hero)
                .foregroundStyle(Color.textError)
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "TBD".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
        .hidesTabBar()
    }
}


// MARK: - Preview
#Preview {
    WorkInProgressView()
}
