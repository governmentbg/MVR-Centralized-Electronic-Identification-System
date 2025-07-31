//
//  MultifactorAuthenticationView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.04.25.
//

import SwiftUI

struct MultifactorAuthenticationView: View {
    @StateObject var viewModel = MultifactorAuthenticationViewModel()
    @EnvironmentObject private var appRootManager: AppRootManager
    @Environment(\.presentationMode) var presentationMode
    var twoFactorResponse: TwoFactorResponse?
    
    var body: some View {
        VStack {
            Spacer()
            EIDTitleView()
            Spacer()
            timerView
            Spacer()
            otpCodeField
            enterButton
            //            newOTPCodeButton
        }
        .padding()
        .addTransparentGradientDividerNavigationBar(content: {
            ToolbarItem(placement: .topBarLeading, content: {
                EIDBackButton()
            })
        })
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .setBackground()
        .onAppear {
            viewModel.twoFactorResponse = twoFactorResponse
            viewModel.startTimer()
        }
        .onDisappear {
            viewModel.stopTimer()
        }
        .onTapGesture {
            hideKeyboard()
        }
        .onChange(of: viewModel.dismissView) { newValue in
            if newValue == true {
                presentationMode.wrappedValue.dismiss()
            }
        }
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
    }
    
    private var otpCodeField: some View {
        EIDInputField(title: "two_factor_otp_code_title".localized(),
                      text: $viewModel.otp.value.max(8),
                      showError: !$viewModel.otp.validation.isValid,
                      errorText: $viewModel.otp.validation.error,
                      isMandatory: true,
                      keyboardType: .numberPad,
                      submitLabel: .next,
                      autocapitalization: .never)
        .padding(.bottom, 32)
    }
    
    private var timerView: some View {
        Text(viewModel.secondsLeft.timeStringFor)
            .font(.heading1)
            .foregroundStyle(Color.themeSecondaryDark)
            .padding()
            .frame(maxWidth: .infinity)
    }
    
    private var enterButton: some View {
        Button(action: {
            viewModel.submitOTP {
                withAnimation(.spring()) {
                    appRootManager.currentRoot = .home
                }
            }
        }, label: {
            Text("btn_proceed".localized())
        })
        .buttonStyle(EIDButton())
    }
    
    //    private var newOTPCodeButton: some View {
    //        Button(action: {
    //            hideKeyboard()
    //            viewModel.generateOTP()
    //        }, label: {
    //            Text("btn_generate_new_otp".localized())
    //        })
    //        .buttonStyle(EIDButton(buttonType: .opaqueOutline,
    //                               buttonState: .primary))
    //        .disabled(viewModel.canGenerateNewOTPCode == false)
    //    }
}

#Preview {
    MultifactorAuthenticationView()
}
