//
//  EmpowermentsToMeDetailsViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.11.23.
//

import Foundation
import Alamofire


final class EmpowermentsToMeDetailsViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var viewState: ViewState = .preview
    @Published var empowerment: Empowerment
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    var screenTitle: String {
        return viewState == .declareDisagreement ? "empowerment_declare_disagreement_title".localized() : "empowerment_preview_title".localized()
    }
    
    // MARK: - Init
    init(empowerment: Empowerment, viewState: ViewState = .preview) {
        self.empowerment = empowerment
        self.viewState = viewState
    }
    
    // MARK: - API calls
    func declareDisagreement() {
        guard let id = empowerment.id else { return }
        showLoading = true
        let request = EmpowermentActionRequest(empowermentId: id)
        EmpowermentsRegisterRouter.declareDisagreement(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success:
                    self?.showSuccess = true
                    self?.successText = "empowerment_disagreement_success_title".localized()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
}

// MARK: - Enums
extension EmpowermentsToMeDetailsViewModel {
    enum ViewState {
        case preview
        case declareDisagreement
    }
}
