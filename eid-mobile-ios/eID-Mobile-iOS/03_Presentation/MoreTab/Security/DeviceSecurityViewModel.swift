//
//  DeviceSecurityViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 12.06.25.
//

import Foundation
import Alamofire


final class DeviceSecurityViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showSigningLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var is2FaEnabled: Bool = false
    @Published var isDevicePINEnabled: Bool = false
    @Published var isBiometricsAvailable: Bool = false
    @Published var showCreatePin: Bool = false
    @Published var pin: String = ""
    @Published var biometricsIsDisabled: Bool = false
    @Published var citizenEID: CitizenEID?
    
    init() {
        isDevicePINEnabled = UserManager.hasDevicePin
        isBiometricsAvailable = UserManager.biometricsAvailable
        biometricsIsDisabled = !isDevicePINEnabled
    }
    
    // MARK: - API calls
    func getCitizenEID() {
        showLoading = true
        ElectronicIdentityRouter.getCitizenEID
            .send(CitizenEID.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let citizenEID):
                    guard let citizenEID = citizenEID else { return }
                    self?.citizenEID = citizenEID
                    self?.is2FaEnabled = citizenEID.is2FaEnabled ?? false
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Methods
    func toggleTwofactorAuth() {
        citizenEID?.is2FaEnabled = is2FaEnabled
        showLoading = true
        ProfileInformationUpdateHelper.change(citizenEID: citizenEID) { [weak self] in
            self?.showLoading = false
        } onFailure: { [weak self] error in
            self?.showLoading = false
            self?.showError = true
            self?.errorText = error.localizedDescription
        }
    }
    
    func toggleDevicePIN() {
        if isDevicePINEnabled {
            showCreatePin = true
        } else {
            UserManager.setDevicePin(pin: nil)
            UserManager.setBiometrics(useBiometrics: false)
            isBiometricsAvailable = false
            biometricsIsDisabled = !isDevicePINEnabled
        }
    }
    
    func toggleBiometricsSecurity() {
        if isBiometricsAvailable {
            BiometricProvider.authenticate { [weak self] success, authenticationError in
                if success {
                    UserManager.setBiometrics(useBiometrics: true)
                } else {
                    self?.isBiometricsAvailable = false
                    self?.biometricsIsDisabled = true
                    self?.showError = true
                    self?.errorText = authenticationError?.description ?? ""
                }
            }
        } else {
            UserManager.setBiometrics(useBiometrics: false)
        }
    }
    
    func devicePINCreated(pin: String) {
        UserManager.setDevicePin(pin: pin)
        isDevicePINEnabled = true
        biometricsIsDisabled = !isDevicePINEnabled
        successText = "create_device_pin_success_text".localized()
        showSuccess = true
    }
}

