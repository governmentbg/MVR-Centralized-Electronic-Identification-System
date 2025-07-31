//
//  ChangeCertificatePINView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 28.06.24.
//

import SwiftUI


struct ChangeCertificatePINView: View {
    // MARK: - Properties
    @Environment(\.presentationMode) var presentationMode
    @StateObject var viewModel = ChangeCertificatePINViewModel()
    @State var device: EIDDevice
    
    // MARK: - Body
    var body: some View {
        ScrollViewReader { reader in
            ScrollView {
                VStack(spacing: 32) {
                    DisclaimerMessageView(title: "eid_card_use_pin_title".localized())
                    oldPinField
                    newPinField
                    confirmNewPinField
                    if viewModel.state == .pinCan {
                        canField
                    }
                    Spacer()
                    changePinButton(reader: reader)
                }
                .padding()
            }
        }
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: "change_card_pin_title".localized(),
                          content: {
            ToolbarItem(placement: .topBarLeading) {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            }
        })
        .onTapGesture {
            hideKeyboard()
        }
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText) {
            presentationMode.wrappedValue.dismiss()
        }
    }
    
    // MARK: - Child views
    private var oldPinField: some View {
        EIDInputField(title: "change_pin_old_pin_title".localized(),
                      text: $viewModel.oldPin.value,
                      showError: !$viewModel.oldPin.validation.isValid,
                      errorText: $viewModel.oldPin.validation.error,
                      isMandatory: true,
                      isPassword: true,
                      keyboardType: .numberPad)
        .limitLength(value: $viewModel.oldPin.value,
                     length: Constants.PIN.length)
        .id(ChangeCertificatePINViewModel.Field.oldPin.rawValue)
    }
    
    private var newPinField: some View {
        EIDInputField(title: "change_pin_new_pin_title".localized(),
                      text: $viewModel.newPin.value,
                      showError: !$viewModel.newPin.validation.isValid,
                      errorText: $viewModel.newPin.validation.error,
                      isMandatory: true,
                      isPassword: true,
                      keyboardType: .numberPad)
        .limitLength(value: $viewModel.newPin.value,
                     length: Constants.PIN.length)
        .id(ChangeCertificatePINViewModel.Field.newPin.rawValue)
    }
    
    private var confirmNewPinField: some View {
        EIDInputField(title: "change_pin_confirm_new_pin_title".localized(),
                      text: $viewModel.confirmNewPin.value,
                      showError: !$viewModel.confirmNewPin.validation.isValid,
                      errorText: $viewModel.confirmNewPin.validation.error,
                      isMandatory: true,
                      isPassword: true,
                      keyboardType: .numberPad)
        .limitLength(value: $viewModel.confirmNewPin.value,
                     length: Constants.PIN.length)
        .id(ChangeCertificatePINViewModel.Field.confirmNewPin.rawValue)
    }
    
    private var canField: some View {
        EIDInputField(title: "change_pin_can_title".localized(),
                      text: $viewModel.can.value,
                      showError: !$viewModel.can.validation.isValid,
                      errorText: $viewModel.can.validation.error,
                      isMandatory: true,
                      isPassword: true,
                      keyboardType: .numberPad)
        .limitLength(value: $viewModel.can.value,
                     length: Constants.PIN.length)
        .id(ChangeCertificatePINViewModel.Field.can.rawValue)
    }
    
    private func changePinButton(reader: ScrollViewProxy) -> some View {
        Button(action: {
            if let errorField = viewModel.firstErrorField {
                reader.scrollTo(errorField.rawValue)
            }
            viewModel.changePin(for: device)
        }, label: {
            Text("change_card_pin_button")
        })
        .buttonStyle(EIDButton())
    }
}


// MARK: - Preview
#Preview {
    ChangeCertificatePINView(device: EIDDevice.default)
}
