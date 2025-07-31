//
//  EmpowermentsFromMeEIKViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 22.05.24.
//

import Foundation

final class EmpowermentsFromMeEIKSearchViewModel: ObservableObject, APICallerViewModel {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showFilter: Bool = false
    @Published var shouldValidateForm: Bool = false
    @Published var showEmpowermentsFromMeView: Bool = false
    /// Input fields
    @Published var eik: BulstatEIK = BulstatEIK(value: "")
    
    // MARK: - Helpers
    func validateFields() -> Bool {
        return eik.validation.isValid
    }
}
