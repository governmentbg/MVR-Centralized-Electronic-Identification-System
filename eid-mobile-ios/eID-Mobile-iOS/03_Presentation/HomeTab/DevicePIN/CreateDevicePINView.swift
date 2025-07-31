//
//  CreateDevicePINView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 11.06.25.
//

import SwiftUI


struct CreateDevicePINView: View {
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
                    .multilineTextAlignment(.center)
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
                Button(action: {
                    hideKeyboard()
                    viewModel.submitPIN()
                }, label: {
                    Text("btn_accept".localized())
                })
                .buttonStyle(EIDButton())
                .disabled(!viewModel.canProceed)
            }
            Spacer()
        }
        .padding()
        .setBackground()
        .onAppear {
            viewModel.clearPIN()
        }
        .onTapGesture {
            hideKeyboard()
        }
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .observeLoading(isLoading: $viewModel.showLoading)
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

#Preview {
    CreateDevicePINView(viewModel: EIDPINViewModel(state: .createDevicePIN, pin: "123456"))
}
