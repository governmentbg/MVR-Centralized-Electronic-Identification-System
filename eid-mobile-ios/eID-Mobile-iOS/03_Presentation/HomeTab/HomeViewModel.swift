//
//  HomeViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 22.05.25.
//

import Foundation
import Alamofire

final class HomeViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var showPinView: Bool = false
    @Published var shouldShowAssociateEIDButton: Bool = false
    
    init() {
        shouldShowAssociateEIDButton = UserManager.getUser()?.eidenityId == nil
    }
    
    // MARK: - API calls
    func associateEid(signedChallenge: SignedChallenge) {
        showLoading = true
        CitizenRouter.associateEid(input:
                                    CitizenAssociateEidRequest(
                                        queryParams: CitizenAssociateEidRequest.QueryParams(),
                                        bodyParams: CitizenAssociateEidRequest.BodyParams(
                                            signature: signedChallenge.signature,
                                            challenge: signedChallenge.challenge,
                                            certificate: signedChallenge.certificate,
                                            certificateChain: signedChallenge.certificateChain)
                                    )
        ).send(Empty.self) { [weak self] response in
            self?.showLoading = false
            switch response {
            case .success:
                self?.shouldShowAssociateEIDButton = false
                self?.showSuccess = true
                self?.successText = "associate_eid_success_message".localized()
            case .failure(let error):
                self?.showError = true
                self?.errorText = error.localizedDescription
            }
        }
    }
}
