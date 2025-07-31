//
//  PINView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 21.05.24.
//

import SwiftUI


struct EIDPINView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @StateObject var viewModel: EIDPINViewModel
    @FocusState private var focusState: Bool
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 64) {
            VStack(spacing: 32) {
                Text(viewModel.state.screenTitle)
                    .font(.heading3)
                    .lineSpacing(4)
                    .foregroundStyle(Color.textDefault)
                Text(viewModel.state.inputTitle)
                    .font(.bodySmall)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textDefault)
                    .fixedSize(horizontal: false, vertical: true)
                    .multilineTextAlignment(.center)
            }
            .padding(.top, 24)
            VStack(spacing: 8) {
                PINField(digitsCount: viewModel.digitsCount,
                         type: .pin,
                         pin: $viewModel.pin)
                if viewModel.state == .loginWithCardCan {
                    canView
                }
                Button(action: {
                    hideKeyboard()
                    viewModel.submitPIN()
                }, label: {
                    Text("btn_proceed".localized())
                })
                .buttonStyle(EIDButton())
                .disabled(!viewModel.canProceed)
            }
            Spacer()
        }
        .padding()
        .setBackground()
        .if(viewModel.state == .loginWithCard,
            transform: { view in
            view.addTransparentGradientDividerNavigationBar(content: {
                ToolbarItem(placement: .topBarLeading) {
                    EIDBackButton()
                }
                ToolbarItem(placement: .topBarTrailing) {
                    Button(action: {
                        viewModel.showInfo = true
                    }, label: {
                        Image("icon_info")
                            .renderingMode(.template)
                            .foregroundColor(Color.buttonDefault)
                    })
                }
            })
        })
        .if(viewModel.state == .loginWithMobileEID,
            transform: { view in
            view.addTransparentGradientDividerNavigationBar(content: {
                ToolbarItem(placement: .topBarLeading) {
                    EIDBackButton()
                }
            })
        })
        .onAppear {
            viewModel.clearPIN()
        }
        .onTapGesture {
            hideKeyboard()
        }
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentInfoView(showInfo: $viewModel.showInfo,
                         title: .constant("info_title".localized()),
                         description: .constant("card_pin_info_description".localized()))
    }
    
    var canView: some View {
        VStack(spacing: 8) {
            Text("eid_use_can_title".localized())
                .font(.bodySmall)
                .lineSpacing(8)
                .foregroundStyle(Color.textDefault)
            PINField(digitsCount: viewModel.digitsCount,
                     type: .can,
                     pin: $viewModel.can)
        }
    }
}


// MARK: - Preview
#Preview {
    EIDPINView(viewModel: EIDPINViewModel(state: .createPIN, pin: "123456"))
}
