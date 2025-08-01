//
//  ContinueApplicationIssueView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 9.01.25.
//

import SwiftUI

struct CompleteEIDApplicationView: View {
    // MARK: - Properties
    @StateObject var viewModel: CompleteEIDApplicationViewModel
    @Environment(\.presentationMode) var presentationMode
    @Binding var path: [String]
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            if viewModel.viewState == .createCertificate {
                Spacer()
                titleView
                    .padding(.bottom, 32)
                ProgressView()
                Spacer()
            } else if viewModel.viewState == .createPin {
                EIDPINView(viewModel: EIDPINViewModel(state: .createPIN,
                                                      pin: viewModel.pin,
                                                      onPINCreate: { didMatch, pin in
                    if didMatch {
                        viewModel.storePinAndCertificate(pin: pin)
                    }
                }))
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .onAppear {
            viewModel.enrollCertificate()
        }
        .addNavigationBar(title: "btn_continue_application".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText, onDismiss: {
            presentationMode.wrappedValue.dismiss()
        })
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {
            path.removeAll()
        })
    }
    
    // MARK: - Child views
    private var titleView: some View {
        Text("continue_application_text".localized())
            .font(.heading4)
            .lineSpacing(8)
            .foregroundStyle(Color.textDefault)
            .padding()
    }
}

#Preview {
    CompleteEIDApplicationView(viewModel: CompleteEIDApplicationViewModel(applicationId: ""),
                               path: .constant([""]))
}
