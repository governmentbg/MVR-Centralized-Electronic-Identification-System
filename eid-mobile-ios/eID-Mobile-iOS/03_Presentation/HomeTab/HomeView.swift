//
//  HomeView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 10.10.23.
//

import SwiftUI


struct HomeView: View {
    // MARK: - Properties
    @StateObject var viewModel = HomeViewModel()
    
    // MARK: - Body
    var body: some View {
        NavigationStack {
            DevicePINView {
                ZStack(alignment: .bottomTrailing) {
                    EIDWebView(type: .home)
                    if viewModel.shouldShowAssociateEIDButton {
                        floatingButtonMenu
                    }
                }
            }
            .background(Color.themeSecondaryLight)
            .addNavigationBar(title: "tab_title_home".localized(),
                              content: {
                ToolbarItem(placement: .topBarTrailing) {
                    EmptyView()
                }
            })
            .sheet(isPresented: $viewModel.showPinView, content: {
                EIDPINView(viewModel: EIDPINViewModel(state: .approveAuthRequest(.cardEID), onAuthRequest: { request in
                    DispatchQueue.main.asyncAfter(deadline: .now() + 1.0) {
                        viewModel.showPinView = false
                        if let signedChallenge = request.signedChallenge {
                            viewModel.associateEid(signedChallenge: signedChallenge)
                        }
                    }
                }))
            })
        }
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText,
                      onDismiss: {
            viewModel.showLoading = false
        })
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText)
    }
    
    private var floatingButtonMenu: some View {
        Button(action: {
            viewModel.showPinView = true
        }, label: {
            Text("associate_eid_button_title".localized().uppercased())
                .lineSpacing(4)
        })
        .buttonStyle(EIDButton(buttonType: .filled,
                               buttonState: .success,
                               wide: false))
        .clipShape(RoundedRectangle(cornerRadius: 20))
        .padding([.bottom, .trailing], 24)
        .addItemShadow()
    }
}


// MARK: - Preview
#Preview {
    HomeView()
}


