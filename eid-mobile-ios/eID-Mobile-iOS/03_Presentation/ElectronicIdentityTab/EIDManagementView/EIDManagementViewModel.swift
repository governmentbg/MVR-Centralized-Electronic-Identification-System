//
//  EIDManagementViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.07.24.
//

import SwiftUI


final class EIDManagementViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showInfo: Bool = false
    /// API Data
    @Published var administrators: [EIDAdministrator] = []
    @Published var devices: [EIDDevice] = []
    @Published var reasons: [Reason] = []
    
    // MARK: - Public methods
    func getHelperData() {
        getAdministrators()
        getDevices()
        getReasons()
    }
    
    // MARK: - API calls
    private func getAdministrators() {
        showLoading = true
        ElectronicIdentityRouter.getAdministrators
            .send(EIDAdministratorsResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let administratorsResponse):
                    guard let administrators = administratorsResponse else { return }
                    self?.administrators = administrators
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func getDevices() {
        showLoading = true
        ElectronicIdentityRouter.getDevices
            .send(EIDDevicesResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let devicesResponse):
                    guard let devices = devicesResponse else { return }
                    self?.devices = devices
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func getReasons() {
        showLoading = true
        ElectronicIdentityRouter.getCertificateActionReasons
            .send(GetReasonsResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let reasons):
                    guard let reasons = reasons else { return }
                    self?.reasons = reasons.stopReasons
                    + reasons.revokeReasons
                    + reasons.resumeReasons
                    + reasons.deniedReasons
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
}
