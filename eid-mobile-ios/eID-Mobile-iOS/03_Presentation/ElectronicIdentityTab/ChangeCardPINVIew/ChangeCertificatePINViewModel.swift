//
//  ChangeCertificatePINViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 28.06.24.
//

import Foundation


@MainActor
final class ChangeCertificatePINViewModel: ObservableObject, @preconcurrency APICallerViewModel, @preconcurrency FieldValidation {
    // MARK: - Properties
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var state = ViewState.pin
    private var eidReader = EIDReader(cardKeyType: .rsa)
    @Published var shouldValidateForm: Bool = false
    // Input fields
    @Published var oldPin: ValidInput = ValidInput(value: "",
                                                   rules: [NotEmptyRule(),
                                                           DigitsOnlyRule(),
                                                           ValidLengthRule(length: Constants.PIN.length)])
    @Published var newPin: CardPin = CardPin(value: "",
                                             isConfirm: false) {
        didSet {
            confirmNewPin.comparePin = newPin.value
        }
    }
    @Published var confirmNewPin: CardPin = CardPin(value: "",
                                                    isConfirm: true)
    @Published var can: CardPin = CardPin(value: "",
                                          isConfirm: false)
    
    // MARK: - Private methods
    func validateFields() -> Bool {
        let isNotValid = oldPin.validation.isNotValid
        || newPin.validation.isNotValid
        || confirmNewPin.validation.isNotValid
        return !isNotValid
    }
    
    // MARK: - Card commands
    func changePin(for device: EIDDevice) {
        shouldValidateForm = true
        guard validateFields() else { return }
        switch device.id {
        case EIDDevice.chipCardID:
            Task { @MainActor in
                do {
                    
                    try await eidReader.changeCardPIN(oldPin: oldPin.value,
                                                      newPin: newPin.value,
                                                      can: can.value.isEmpty ? nil : can.value)
                    showSuccessMessage()
                } catch let error {
                    if let cardError = error as? NFCEIDCardReaderError, case NFCEIDCardReaderError.CardSuspended = cardError {
                        clearFields()
                        state = .pinCan
                    }
                }
            }
        case EIDDevice.mobileDeviceID:
            if let certificatePinError = CertificateManager.changeCertificatePin(oldPin: oldPin.value, newPin: newPin.value) {
                showErrorMessage(error: certificatePinError.errorDescription)
            } else {
                showSuccessMessage()
            }
        default: return
        }
    }
    
    private func showSuccessMessage() {
        showSuccess = true
        successText = "change_certificate_pin_success_title".localized()
    }
    
    private func showErrorMessage(error: String?) {
        showError = true
        errorText = error ?? "change_certificate_pin_error_title".localized()
    }
    
    private func clearFields () {
        oldPin.value = ""
        newPin.value = ""
        confirmNewPin.value = ""
        can.value = ""
    }
}


extension ChangeCertificatePINViewModel {
    enum InputFieldType: CaseIterable {
        case oldPin
        case newPin
        case confirmNewPin
    }
}

extension ChangeCertificatePINViewModel {
    enum ViewState {
        case pin, pinCan
    }
}

extension ChangeCertificatePINViewModel {
    // MARK: - Fields
    enum Field: Int, CaseIterable {
        case oldPin
        case newPin
        case confirmNewPin
        case can
    }
    
    func inputFieldValue(field: Field) -> Validation {
        switch field {
        case .oldPin:
            oldPin
        case .newPin:
            newPin
        case .confirmNewPin:
            confirmNewPin
        case .can:
            can
        }
    }
}
