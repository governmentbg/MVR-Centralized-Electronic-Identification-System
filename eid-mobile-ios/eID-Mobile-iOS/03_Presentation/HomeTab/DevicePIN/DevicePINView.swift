//
//  DevicePINView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 11.06.25.
//

import SwiftUI


struct DevicePINView<Content: View>: View {
    // MARK: - Properties
    @ViewBuilder let content: Content
    @StateObject var viewModel = DevicePINViewModel()
    
    // MARK: - Body
    var body: some View {
        content
            .onAppear {
                DispatchQueue.main.asyncAfter(deadline: .now() + 1.5) {
                    let hasSetupPin = UserDefaults.standard.bool(forKey: Constants.UserDefaultsKeys.hasSetupPin)
                    if !hasSetupPin && UserManager.hasDevicePin == false {
                        viewModel.showPinPrompt = true
                    }
                }
            }
            .presentAlert(showAlert: $viewModel.showSuccess,
                          alertText: $viewModel.successText,
                          onDismiss: {
                viewModel.showCreatePin = false
            })
            .presentMultipleButtonAlert(showAlert: $viewModel.showPinPrompt,
                                        alertText: $viewModel.pinAlertTitle,
                                        buttons: viewModel.pinAlertButtons)
            .sheet(isPresented: $viewModel.showCreatePin, content: {
                CreateDevicePINView(viewModel: EIDPINViewModel(state: .createDevicePIN,
                                                               pin: viewModel.pin,
                                                               onPINCreate: { didMatch, pin in
                    if didMatch {
                        viewModel.devicePINCreated(pin: pin)
                    }
                }))
            })
    }
}
