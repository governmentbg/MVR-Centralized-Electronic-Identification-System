//
//  ChangePhoneViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 19.06.24.
//

import Foundation


final class ChangeInformationViewModel: ObservableObject, APICallerViewModel, FieldValidation {
    // MARK: - Properties
    var citizenEid: CitizenEID?
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var showInfo: Bool = false
    @Published var shouldValidateForm: Bool = false
    /// Input field
    @Published var firstName = Name(value: "",
                                    isLatin: false,
                                    firstCapitalValidation: true,
                                    isMandatory: true)
    @Published var secondName = Name(value: "",
                                     isLatin: false,
                                     isCompoundName: true) {
        didSet {
            if lastName.otherCompoundName != secondName.value {
                lastName.otherCompoundName = secondName.value
            }
        }
    }
    @Published var lastName = Name(value: "",
                                   isLatin: false,
                                   isCompoundName: true) {
        didSet {
            if secondName.otherCompoundName != lastName.value {
                secondName.otherCompoundName = lastName.value
            }
        }
    }
    @Published var firstNameLatin = Name(value: "",
                                         isLatin: true,
                                         firstCapitalValidation: true,
                                         isMandatory: true)
    @Published var secondNameLatin = Name(value: "",
                                          isLatin: true,
                                          isCompoundName: true) {
        didSet {
            if lastNameLatin.otherCompoundName != secondNameLatin.value {
                lastNameLatin.otherCompoundName = secondNameLatin.value
            }
        }
    }
    @Published var lastNameLatin = Name(value: "",
                                        isLatin: true,
                                        isCompoundName: true) {
        didSet {
            if secondNameLatin.otherCompoundName != lastNameLatin.value {
                secondNameLatin.otherCompoundName = lastNameLatin.value
            }
        }
    }
    @Published var phone = Phone(value: "",
                                 validation: .valid)
    
    init(citizenEid: CitizenEID?) {
        self.citizenEid = citizenEid
        setupFields()
    }
    
    // MARK: - Private methods
    func validateFields() -> Bool {
        if lastName.otherCompoundName?.isEmpty == true {
            lastName.otherCompoundName = secondName.value
        }
        
        if secondName.otherCompoundName?.isEmpty == true {
            secondName.otherCompoundName = lastName.value
        }
        
        if lastNameLatin.otherCompoundName?.isEmpty == true {
            lastNameLatin.otherCompoundName = secondNameLatin.value
        }
        
        if secondNameLatin.otherCompoundName?.isEmpty == true {
            secondNameLatin.otherCompoundName = lastNameLatin.value
        }
        
        return firstName.validation.isValid &&
        secondName.validation.isValid &&
        lastName.validation.isValid &&
        firstNameLatin.validation.isValid &&
        secondNameLatin.validation.isValid &&
        lastNameLatin.validation.isValid &&
        phone.validation.isValid
    }
    
    private func setupFields() {
        if let citizenEid {
            firstName.value = citizenEid.firstName ?? ""
            secondName.value = citizenEid.secondName ?? ""
            lastName.value = citizenEid.lastName ?? ""
            firstNameLatin.value = citizenEid.firstNameLatin ?? ""
            secondNameLatin.value = citizenEid.secondNameLatin ?? ""
            lastNameLatin.value = citizenEid.lastNameLatin ?? ""
            phone.value = citizenEid.phoneNumber ?? ""
        }
    }
    
    // MARK: - API calls
    func changePhone() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let citizenUpdateInfo = CitizenEID(eidentityId: nil,
                                           citizenProfileId: nil,
                                           active: nil,
                                           firstName: firstName.value,
                                           secondName: secondName.value,
                                           lastName: lastName.value,
                                           firstNameLatin: firstNameLatin.value,
                                           secondNameLatin: secondNameLatin.value,
                                           lastNameLatin: lastNameLatin.value,
                                           phoneNumber: phone.value,
                                           is2FaEnabled: citizenEid?.is2FaEnabled ?? false)
        showLoading = true
        ProfileInformationUpdateHelper.change(citizenEID: citizenUpdateInfo) { [weak self] in
            self?.showLoading = false
            self?.showSuccess = true
            self?.successText = "change_information_success_title".localized()
        } onFailure: { [weak self] error in
            self?.showLoading = false
            self?.showError = true
            self?.errorText = error.localizedDescription
        }
    }
}

extension ChangeInformationViewModel {
    // MARK: - Fields
    enum Field: Int, CaseIterable {
        case firstName
        case secondName
        case lastName
        case firstNameLatin
        case secondNameLatin
        case lastNameLatin
        case phone
    }
    
    func inputFieldValue(field: Field) -> Validation {
        switch field {
        case .firstName:
            firstName
        case .secondName:
            secondName
        case .lastName:
            lastName
        case .firstNameLatin:
            firstNameLatin
        case .secondNameLatin:
            secondNameLatin
        case .lastNameLatin:
            lastNameLatin
        case .phone:
            phone
        }
    }
}
