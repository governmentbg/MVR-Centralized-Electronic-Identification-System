//
//  CompleteApplicationView.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 7.05.24.
//

import SwiftUI


struct CompleteApplicationView: View {
    // MARK: - Properties
    @StateObject var viewModel = CompleteApplicationViewModel()
    @Environment(\.presentationMode) var presentationMode
    
    // MARK: - Body
    var body: some View {
        VStack(spacing: 0) {
            if viewModel.viewState == .scanQR {
                QRScanerView(onCodeScanned: { scannedCode in
                    viewModel.processScannedCode(scannedCode)
                })
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
        .addNavigationBar(title: "",
                          content: {
            ToolbarItem(placement: .topBarLeading, content: {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            })
        })
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText,
                      onDismiss: { presentationMode.wrappedValue.dismiss() })
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: { presentationMode.wrappedValue.dismiss() })
    }
}
