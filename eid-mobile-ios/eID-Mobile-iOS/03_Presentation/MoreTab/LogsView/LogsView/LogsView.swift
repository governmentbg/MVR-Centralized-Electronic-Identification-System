//
//  LogsView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import SwiftUI


struct LogsView: View {
    // MARK: - Properties
    @StateObject var viewModel = LogsViewModel()
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        VStack {
            NavigationLink(destination: LogsFromMeView(viewModel:
                                                        LogsFromMeViewModel(viewState: .fromMe, logsDescriptions: viewModel.localisedDescriptions)),
                           label: {
                ListMenuItem(imageName: LogDirection.fromMe.iconName,
                             title: LogDirection.fromMe.title.localized(),
                             subtitle: LogDirection.fromMe.subtitle.localized())
            }).buttonStyle(ListMenuButtonStyle())
            NavigationLink(destination: LogsToMeView(viewModel:
                                                        LogsToMeViewModel(viewState: .toMe, logsDescriptions: viewModel.localisedDescriptions)),
                           label: {
                ListMenuItem(imageName: LogDirection.toMe.iconName,
                             title: LogDirection.toMe.title.localized(),
                             subtitle: LogDirection.toMe.subtitle.localized())
            }).buttonStyle(ListMenuButtonStyle())
            Spacer()
        }
        .padding()
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "logs_screen_title".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
        .onAppear {
            viewModel.getLogLocalisations()
        }
    }
}
