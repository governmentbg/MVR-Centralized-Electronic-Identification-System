//
//  DevicePINViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 11.06.25.
//

import Foundation


final class DevicePINViewModel: ObservableObject {
    // MARK: - Properties
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var showPinPrompt: Bool = false
    @Published var pinAlertTitle: String = "device_pin_alert_title".localized()
    var pinAlertButtons: [AlertButton] {
        return [
            AlertButton(title: "btn_no".localized(), action: { [weak self] in
                self?.setPinAlertShown()
            }),
            AlertButton(title: "btn_yes".localized(), action: { [weak self] in
                self?.setPinAlertShown()
                self?.showCreatePin = true
            })
        ]
    }
    @Published var showCreatePin: Bool = false
    @Published var pin: String = ""
    
    // MARK: - Methods
    func devicePINCreated(pin: String) {
        UserManager.setDevicePin(pin: pin)
        successText = "create_device_pin_success_text".localized()
        showSuccess = true
    }
    
    private func setPinAlertShown() {
        let hasSetupPin = UserDefaults.standard.bool(forKey: Constants.UserDefaultsKeys.hasSetupPin)
        if !hasSetupPin {
            UserDefaults.standard.set(true, forKey: Constants.UserDefaultsKeys.hasSetupPin)
        }
    }
}
