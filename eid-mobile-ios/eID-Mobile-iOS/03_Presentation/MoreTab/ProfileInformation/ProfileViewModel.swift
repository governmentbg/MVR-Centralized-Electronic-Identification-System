//
//  ProfileViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 19.06.24.
//

import Foundation
import Alamofire


final class ProfileViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showSigningLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var citizenEID: CitizenEID?
    @Published var handleAction: Bool = false
    @Published var showPinView: Bool = false
    @Published var selectedAction: ProfileAction = .noAction
    @Published var is2FaEnabled: Bool = false
    
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
    
    func goToChangeInformation() {
        selectedAction = .changeInformation
        handleAction = true
    }
    
    func goToChangePassword() {
        selectedAction = .chanhgePassword
        handleAction = true
    }
    
    func goToChangeEmail() {
        selectedAction = .changeEmail
        handleAction = true
    }
    
    func toCertificatePinView() {
        showPinView = true
    }
}

extension ProfileViewModel {
    enum ProfileAction {
        case noAction
        case changeInformation
        case chanhgePassword
        case changeEmail
        case associateId
    }
}
