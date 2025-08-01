//
//  DeviceSecurityView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 12.06.25.
//

import SwiftUI


struct DeviceSecurityView: View {
    // MARK: - Properteis
    @StateObject var viewModel = DeviceSecurityViewModel()
    @Environment(\.presentationMode) var presentationMode
    // MARK: - Private Properties
    private var padding: CGFloat = 32
    
    // MARK: - Body
    var body: some View {
        ScrollView {
            VStack(spacing: padding) {
                authSecuritySection
                if UserManager.getUser()?.acr == .low {
                    deviceSecuritySection
                }
            }
            .padding([.leading, .trailing, .top], padding)
        }
        .onAppear {
            viewModel.getCitizenEID()
        }
        .background(Color.themeSecondaryLight)
        .addNavigationBar(title: MoreMenuOption.security.localizedTitle(),
                          content: {
            ToolbarItem(placement: .topBarLeading) {
                Button(action: {
                    presentationMode.wrappedValue.dismiss()
                }, label: {
                    Image("icon_arrow_left")
                })
            }
        })
        .observeLoading(isLoading: $viewModel.showLoading)
        .presentAlert(showAlert: $viewModel.showError,
                      alertText: $viewModel.errorText)
        .presentAlert(showAlert: $viewModel.showSuccess,
                      alertText: $viewModel.successText,
                      onDismiss: {})
        .sheet(isPresented: $viewModel.showCreatePin, onDismiss: {
            viewModel.isDevicePINEnabled = UserManager.hasDevicePin
        }, content: {
            CreateDevicePINView(viewModel: EIDPINViewModel(state: .createDevicePIN,
                                                           pin: viewModel.pin,
                                                           onPINCreate: { didMatch, pin in
                if didMatch {
                    viewModel.devicePINCreated(pin: pin)
                }
            }))
        })
    }
    
    private func section(title: String) -> some View {
        VStack {
            Text(title)
                .font(.heading6)
                .lineSpacing(10)
                .foregroundStyle(Color.textDefault)
                .frame(maxWidth: .infinity, alignment: .leading)
            GradientDivider()
        }
    }
    
    private var authSecuritySection: some View {
        VStack {
            section(title: "profile_section_multifactor_authentication_title".localized().uppercased())
            Toggle(isOn: $viewModel.is2FaEnabled.didSet(execute: { _ in
                viewModel.toggleTwofactorAuth()
            })) {
                VStack(alignment: .leading, spacing: 8) {
                    Text("device_pin_toggle_title".localized())
                        .font(.bodyRegular)
                        .lineSpacing(8)
                        .foregroundStyle(Color.textDefault)
                        .multilineTextAlignment(.leading)
                }
            }
            .tint(Color.buttonDefault)
        }
    }
    
    private var deviceSecuritySection: some View {
        VStack {
            section(title: "device_security_menu_option".localized().uppercased())
            devicePINToggle
            biometricsToggle
        }
    }
    
    private var devicePINToggle: some View {
        Toggle(isOn: $viewModel.isDevicePINEnabled.didSet(execute: { _ in
            viewModel.toggleDevicePIN()
        })) {
            VStack(alignment: .leading, spacing: 8) {
                Text("device_pin_login_button_title".localized())
                    .font(.bodyRegular)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textDefault)
                    .multilineTextAlignment(.leading)
            }
        }
        .tint(Color.buttonDefault)
    }
    
    private var biometricsToggle: some View {
        Toggle(isOn: $viewModel.isBiometricsAvailable.didSet(execute: { _ in
            viewModel.toggleBiometricsSecurity()
        })) {
            VStack(alignment: .leading, spacing: 8) {
                Text(BiometricProvider.biometricType.description)
                    .font(.bodyRegular)
                    .lineSpacing(8)
                    .foregroundStyle(Color.textDefault)
                    .multilineTextAlignment(.leading)
            }
        }
        .tint(Color.buttonDefault)
        .disabled(viewModel.biometricsIsDisabled)
    }
}

#Preview {
    DeviceSecurityView()
}
