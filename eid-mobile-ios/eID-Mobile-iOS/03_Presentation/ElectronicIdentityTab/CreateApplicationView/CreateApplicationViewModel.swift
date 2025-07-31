//
//  CreateApplicationViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 1.03.24.
//

import Foundation
import Alamofire


final class CreateApplicationViewModel: SignatureProviderHelper, FieldValidation {
    // MARK: - Properties
    @Published var formState: FormState = .edit
    @Published var certificateActionsInfo = CertificateDetailsActionModel()
    @Published var administrators: [EIDAdministrator]
    @Published var devices: [EIDDevice]
    /// Flags
    @Published var showAdministratorsList: Bool = false
    @Published var showOfficesList: Bool = false
    @Published var showDevicesList: Bool = false
    @Published var showBirthDatePicker: Bool = false
    @Published var showIssueDatePicker: Bool = false
    @Published var showValidToDatePicker: Bool = false
    @Published var isOnlineOffice: Bool = false
    @Published var showPaymentView: Bool = false
    @Published var shouldValidateForm: Bool = false
    /// Dates
    @Published var birthDate = Date()
    @Published var issueDate = Date()
    @Published var validToDate = Date()
    /// Data to send
    @Published var firstName: Name = Name(value: "", isLatin: false, firstCapitalValidation: true, isMandatory: true)
    @Published var secondName: Name = Name(value: "", isLatin: false)
    @Published var lastName: Name = Name(value: "", isLatin: false)
    @Published var firstNameLatin: Name = Name(value: "", isLatin: true, firstCapitalValidation: true, isMandatory: true)
    @Published var secondNameLatin: Name = Name(value: "", isLatin: true)
    @Published var lastNameLatin: Name = Name(value: "", isLatin: true)
    @Published var dateOfBirth: ValidInput = ValidInput(value: "", rules: [NotEmptyRule(), IsAdultRule()])
    @Published var citizenIdentifierTypeStr: String = ""
    @Published var applicationType: EIDApplicationType = .issue
    @Published var citizenship: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var identityNumber: ValidInput = ValidInput(value: "",
                                                           rules: [NotEmptyRule(),
                                                                   ValidLengthRule(length: 9),
                                                                   ValidIdDocumentNumberRule()])
    @Published var citizenIdentifier = PersonalID(idType: .egn,
                                                  idTypeText: "",
                                                  id: "",
                                                  name: Name(value: "", isLatin: false),
                                                  visible: false)
    @Published var identityType: IdentityType = .identityCard {
        didSet {
            identityTypeStr.value = identityType.title.localized()
        }
    }
    @Published var identityTypeStr: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var identityIssueDate: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var identityValidityToDate: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var email: String = ""
    var fullName: String {
        return [firstName.value, secondName.value, lastName.value].fullName
    }
    /// Input fields
    @Published var signatureProviderText: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    /// API data
    var offices: [EIDAdministratorOffice] = []
    @Published var identityTypes = IdentityType.allCases
    private var citizenEID: CitizenEID?
    var reasons: [Reason] = [] {
        didSet {
            reason.value = reasons.first?.description ?? ""
        }
    }
    var applicationPayment: [Payment]?
    /// Display data
    @Published var administratorItems: [PickerViewItem] = []
    @Published var deviceItems: [EIDDevice] = []
    @Published var officeItems: [PickerViewItem] = []
    @Published var reason: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var showCustomReason: Bool = false
    @Published var customReason: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    var screenTitle: String {
        "application_screen_title".localized()
    }
    var previewStateButtonTitle: String {
        UserManager.getUser()?.acr == .high ? "btn_submit".localized() : "btn_sign".localized()
    }
    var viewTitle: String {
        switch formState {
        case .stopCertificate:
            return EIDApplicationType.stop.screenTitle.localized()
        case .resumeCertificate:
            return EIDApplicationType.resume.screenTitle.localized()
        case .revokeCertificate:
            return EIDApplicationType.revoke.screenTitle.localized()
        default:
            return "create_application_title".localized()
        }
    }
    var certificateActionButtonTitle: String {
        switch formState {
        case .stopCertificate:
            return "certificate_btn_confirm_stop".localized()
        case .resumeCertificate:
            return "certificate_btn_confirm_resume".localized()
        case .revokeCertificate:
            return "certificate_btn_confirm_revoke".localized()
        default: return ""
        }
    }
    var shouldShowReasonField: Bool {
        return formState == .stopCertificate || formState == .revokeCertificate
    }
    /// Data to send
    @Published var selectedAdministrator: EIDAdministrator = .default {
        didSet {
            if selectedAdministrator.validation.isValid {
                setDeviceItems()
                getOffices()
            }
        }
    }
    @Published var selectedDevice: EIDDevice = .default {
        didSet {
            if selectedDevice.validation.isValid {
                setOfficeItems()
                updateDocumentNumberValidations()
            }
        }
    }
    @Published var selectedOffice: EIDAdministratorOffice = .default
    /// Signing
    private var detachedSignature: String? = nil
    
    // MARK: - Init
    init(administrators: [EIDAdministrator], devices: [EIDDevice]) {
        self.administrators = administrators
        self.devices = devices
    }
    
    // MARK: - Public methods
    func validateAndPreview() {
        shouldValidateForm = true
        guard validateFields() else { return }
        generateApplicationXml()
    }
    
    func submitApplication() {
        guard validateFields(),
              !xmlBase64.isEmpty
        else { return }
        if UserManager.getUser()?.acr == .high {
            createApplication()
        } else {
            startSigningProcess(uid: citizenIdentifier.id)
        }
    }
    
    func getInitalData() {
        getCitizenEID()
        if !formState.isCertificateState {
            getAdministrators()
        }
        fileName = "\(applicationType.title.localized())\(Date().toLocalTime().normalizeDate(outputFormat: .iso8601DateTime))"
        didSuccessfullyDownloadSignature = { [weak self] signature in
            self?.detachedSignature = signature
            if self?.formState.shouldSignCertificateChange == true {
                self?.changeCertificateStatusSigned()
            } else {
                self?.createApplication()
            }
        }
        setSignatureProvider(.evrotrust)
        identityType = .identityCard
    }
    
    // MARK: - API calls
    private func getCitizenEID() {
        showLoading = true
        ElectronicIdentityRouter.getCitizenEID
            .send(CitizenEID.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let citizenEID):
                    guard let citizenEID = citizenEID else { return }
                    self?.citizenEID = citizenEID
                    self?.setCitizenData()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func getAdministrators() {
        showLoading = true
        ElectronicIdentityRouter.getAdministrators
            .send(EIDAdministratorsResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let administrators):
                    guard let administrators = administrators else { return }
                    self?.administrators = administrators
                    self?.setAdministratorItems()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func getOffices() {
        guard !selectedAdministrator.id.isEmpty else { return }
        showLoading = true
        let request = GetDataById(id: selectedAdministrator.id)
        ElectronicIdentityRouter.getOfficeByAdministratorId(input: request)
            .send(EIDAdministratorOfficesResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let officesResponse):
                    guard let offices = officesResponse else { return }
                    self?.offices = offices
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func generateApplicationXml() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let selectedReason = reasons.filter({ $0.description == reason.value }).first
        let request = GenerateApplicationXmlRequest(firstName: firstName.value,
                                                    secondName: secondName.value,
                                                    lastName: lastName.value,
                                                    firstNameLatin: firstNameLatin.value,
                                                    secondNameLatin: secondNameLatin.value,
                                                    lastNameLatin: lastNameLatin.value,
                                                    applicationType: formState.newEIDApplicationType,
                                                    deviceId: formState.shouldSignCertificateChange ? certificateActionsInfo.deviceId : selectedDevice.id,
                                                    citizenship: citizenship.value,
                                                    citizenIdentifierNumber: citizenIdentifier.id,
                                                    citizenIdentifierType: citizenIdentifier.idType,
                                                    personalIdentityDocument: PersonalIdentityDocument(identityNumber: identityNumber.value.uppercased(),
                                                                                                       identityType: identityTypeStr.value,
                                                                                                       identityIssueDate: getISODate(identityIssueDate.value),
                                                                                                       identityValidityToDate: getISODate(identityValidityToDate.value)),
                                                    eidAdministratorId: formState.shouldSignCertificateChange ? certificateActionsInfo.eidAdministratorId : selectedAdministrator.id,
                                                    eidAdministratorOfficeId: formState.shouldSignCertificateChange ? certificateActionsInfo.eidAdministratorOfficeId : selectedOffice.id,
                                                    dateOfBirth: getISODate(dateOfBirth.value),
                                                    reasonId: selectedReason?.id ?? nil,
                                                    reasonText: (selectedReason?.id != nil && showCustomReason) ? customReason.value : nil,
                                                    certificateId: formState.shouldSignCertificateChange ? certificateActionsInfo.certificateId : nil)
        ElectronicIdentityRouter.generateApplicationXml(input: request)
            .send(GenerateApplicationXmlResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let xmlResponse):
                    guard let xmlResponse = xmlResponse else { return }
                    self?.xmlBase64 = xmlResponse.xml.toBase64()
                    if self?.formState.shouldSignCertificateChange == true {
                        self?.submitApplication()
                    } else {
                        self?.changeState(to: .preview)
                    }
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func createApplication() {
        let shouldSign = UserManager.getUser()?.acr != .high
        let request = CreateApplicationRequest(xml: xmlBase64,
                                               signature: shouldSign ? detachedSignature : nil,
                                               signatureProvider: shouldSign ? signatureProvider.rawValue : nil)
        ElectronicIdentityRouter.createApplication(input: request)
            .send(CreateApplicationResponse.self) { [weak self] response in
                guard let self = self else { return }
                self.showLoading = false
                switch response {
                case .success(let application):
                    if application?.status == .pendingPayment {
                        self.applicationPayment = application?.payment
                        self.showPaymentView = true
                    } else {
                        var successMessage = "application_success_title".localized()
                        if self.selectedOffice.id != Constants.Administrator.Office.onlineOfficeId {
                            successMessage = "application_success_title_in_office".localized()
                        }
                        if self.selectedDevice.id == EIDDevice.chipCardID
                            && self.selectedAdministrator.id == Constants.Administrator.Office.mvrAdministratorId {
                            successMessage.append("\n" + "application_success_title_for_id_card".localized())
                        }
                        self.showSuccess = true
                        self.successText = successMessage
                    }
                case .failure(let error):
                    self.showError = true
                    self.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Public Helpers
    func changeState(to newState: FormState) {
        formState = newState
    }
    
    func setBirthDate() {
        dateOfBirth.value = birthDate.toLocalTime().normalizeDate(outputFormat: .iso8601Date)
    }
    
    func setIssueDate() {
        identityIssueDate.value = issueDate.toLocalTime().normalizeDate(outputFormat: .iso8601Date)
    }
    
    func setValidToDate() {
        identityValidityToDate.value = validToDate.toLocalTime().normalizeDate(outputFormat: .iso8601Date)
    }
    
    func setSignatureProvider(_ provider: SignatureProvider) {
        signatureProvider = provider
        signatureProviderText.value = provider.title.localized()
    }
    
    func setIdFieldType(_ idType: IdentifierType) {
        citizenIdentifier.id = ""
        citizenIdentifier.idType = idType
    }
    
    func clearBirthDate() {
        birthDate = .now
        dateOfBirth.value = ""
    }
    
    func clearIssueDate() {
        issueDate = .now
        identityIssueDate.value = ""
    }
    
    func clearValidToDate() {
        identityValidityToDate.value = ""
        validToDate = .now
    }
    
    // MARK: - Private Helpers
    private func setCitizenData() {
        guard let eid = citizenEID else { return }
        firstName.value = eid.firstName ?? ""
        secondName.value = eid.secondName ?? ""
        lastName.value = eid.lastName ?? ""
        firstNameLatin.value = eid.firstNameLatin ?? ""
        secondNameLatin.value = eid.secondNameLatin ?? ""
        lastNameLatin.value = eid.lastNameLatin ?? ""
        citizenIdentifier.idType = eid.citizenIdentifierType ?? .egn
        citizenIdentifier.id = eid.citizenIdentifierNumber ?? ""
        email = citizenEID?.email ?? ""
#if DEBUG
        teddyTestData()
//        steliyoTestData()
#endif
    }
    
    private func setAdministratorItems() {
        administratorItems = administrators.map { PickerViewItem(id: $0.id, name: $0.name) }
    }
    
    private func setDeviceItems() {
        let administratorDeviceIds = administrators.first(where: { $0.id == selectedAdministrator.id })?.deviceIds ?? []
        selectedDevice = .default
        deviceItems = devices.filter({ administratorDeviceIds.contains($0.id) })
    }
    
    private func setOfficeItems() {
        guard let selectedDevice = deviceItems.first(where: { $0.id == selectedDevice.id }) else {
            officeItems = []
            return
        }
        /// TODO: HIGH ACR REMOVED FOR TESTING PURPOSES. REMOVE BEFORE RELEASE!!!
        //        if selectedDevice.type.contains("MOBILE"),
        //           let userAcr = UserManager.getUser()?.acr,
        //           userAcr == .high,
        //           let onlineOffice = offices.first(where: { $0.id == Constants.Administrator.Office.onlineOfficeId && devices.contains(selectedDevice) }) {
        //            officeItems = [PickerViewItem(id: onlineOffice.id,
        //                                          name: onlineOffice.name)]
        //            selectedOffice = onlineOffice
        //            isOnlineOffice = true
        //        } else {
        //            officeItems = offices
        //                .filter({ $0.id != Constants.Administrator.Office.onlineOfficeId && devices.contains(selectedDevice) })
        //                .map({ PickerViewItem(id: $0.id, name: $0.name)})
        //            selectedOffice = .default
        //            isOnlineOffice = false
        //        }
        
        officeItems = offices
            .filter({ _ in devices.contains(selectedDevice) })
            .map({ PickerViewItem(id: $0.id, name: $0.name)})
        selectedOffice = .default
    }
    
    private func updateDocumentNumberValidations() {
        if (selectedDevice.id == EIDDevice.chipCardID && !formState.isCertificateState)
            || UserManager.getUser()?.acr == .high  {
            identityNumber.rules = [NotEmptyRule(),
                                    ValidLengthRule(length: 9),
                                    ValidNewIdDocumentNumberRule()]
        } else {
            identityNumber.rules = [NotEmptyRule(),
                                    ValidLengthRule(length: 9),
                                    ValidIdDocumentNumberRule()]
        }
    }
    
    private func clearFields() {
        firstName.value = ""
        secondName.value = ""
        lastName.value = ""
        firstNameLatin.value = ""
        secondNameLatin.value = ""
        lastNameLatin.value = ""
        citizenship.value = ""
        citizenIdentifier.id = ""
        citizenIdentifier.idType = .egn
        dateOfBirth.value = ""
        identityType = .identityCard
        identityNumber.value = ""
        identityIssueDate.value = ""
        identityValidityToDate.value = ""
        selectedAdministrator = .default
        selectedDevice = .default
        selectedOffice = .default
    }
    
    func validateFields() -> Bool {
        let nameisValid = (firstName.validation.isValid || secondName.value.isValid(rules: [NotEmptyRule()]).isValid)
        && (firstName.validation.isValid || lastName.value.isValid(rules: [NotEmptyRule()]).isValid)
        let latinNameisValid = (firstNameLatin.validation.isValid || secondNameLatin.value.isValid(rules: [NotEmptyRule()]).isValid)
        && (firstNameLatin.validation.isValid || lastNameLatin.value.isValid(rules: [NotEmptyRule()]).isValid)
        
        guard nameisValid,
              latinNameisValid,
              dateOfBirth.validation.isValid,
              citizenIdentifier.validation.isValid,
              citizenship.validation.isValid,
              identityNumber.validation.isValid,
              identityIssueDate.validation.isValid,
              identityValidityToDate.validation.isValid
        else {
            return false
        }
        
        switch formState {
        case .stopCertificate:
            return showCustomReason ? customReason.validation.isValid : reason.validation.isValid
        case .resumeCertificate:
            return true
        case .revokeCertificate:
            let isReasonValid = showCustomReason ? customReason.validation.isValid : reason.validation.isValid
            return isReasonValid
        default:
            let isApplicationValid = selectedAdministrator.validation.isValid == true
            && selectedDevice.validation.isValid
            && selectedOffice.validation.isValid
            return isApplicationValid
        }
    }
    
    private func showTryAgainError() {
        showLoading = false
        showError = true
        errorText = "error_message_try_again".localized()
    }
    
    func setReason(_ newReason: String?, isOtherSelected: Bool = false) {
        showCustomReason = isOtherSelected
        reason.value = newReason ?? ""
        customReason.value = ""
    }
    
    func getISODate(_ date: String) -> String {
        return date.toDate(withFormats: [.iso8601Date])?.normalizeDate(outputFormat: .iso8601Date) ?? ""
    }
    
    /// Certificate actions
    
    func changeCertificateStatus() {
        guard let acr = UserManager.getUser()?.acr else { return }
        if acr == .high || acr == .substantial || (acr == .low && formState == .stopCertificate) {
            changeCertificateStatusPlain()
        } else {
            generateApplicationXml()
        }
    }
    
    func changeCertificateStatusPlain() {
        shouldValidateForm = true
        guard validateFields() else { return }
        showLoading = true
        let newStatus = formState.newEIDApplicationType
        let selectedReason = reasons.filter({ $0.description == reason.value }).first
        let params = ChangeCertificateStatusRequest(firstName: firstName.value,
                                                    secondName: secondName.value,
                                                    lastName: lastName.value,
                                                    firstNameLatin: firstNameLatin.value,
                                                    secondNameLatin: secondNameLatin.value,
                                                    lastNameLatin: lastNameLatin.value,
                                                    dateOfBirth: dateOfBirth.value,
                                                    eidAdministratorId: certificateActionsInfo.eidAdministratorId,
                                                    eidAdministratorOfficeId: certificateActionsInfo.eidAdministratorOfficeId,
                                                    citizenIdentifierNumber: citizenIdentifier.id,
                                                    citizenIdentifierType: citizenIdentifier.idType,
                                                    applicationType: newStatus,
                                                    deviceType: identityType.rawValue,
                                                    citizenship: citizenship.value,
                                                    personalIdentityDocument: PersonalIdentityDocument(
                                                        identityNumber: identityNumber.value,
                                                        identityType: identityTypeStr.value,
                                                        identityIssueDate: getISODate(identityIssueDate.value),
                                                        identityValidityToDate: getISODate(identityValidityToDate.value)),
                                                    reasonId: selectedReason?.id ?? nil,
                                                    reasonText: showCustomReason ? customReason.value : nil,
                                                    certificateId: certificateActionsInfo.certificateId)
        ElectronicIdentityRouter.changeCertificateStatusPlain(input: params)
            .send(CreateApplicationResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let response):
                    guard let response = response else { return }
                    self?.certificateStatusChangeResponseHandler(response: response)
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func changeCertificateStatusSigned() {
        guard let signature = detachedSignature else { return }
        let request = CreateApplicationRequest(xml: xmlBase64,
                                               signature: signature,
                                               signatureProvider: signatureProvider.rawValue)
        ElectronicIdentityRouter.changeCertificateStatusSigned(input: request)
            .send(CreateApplicationResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let response):
                    guard let response = response else { return }
                    self?.certificateStatusChangeResponseHandler(response: response)
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func completeApplicationStatusChange(applicationId: String) {
        showLoading = true
        let request = CompleteCertificateStatusChangeRequest(applicationId: applicationId)
        ElectronicIdentityRouter.completeCertificateStatusChange(input: request)
            .send(Empty.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(_):
                    self?.showSuccess = true
                    self?.successText = self?.formState.successMessage.localized() ?? ""
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func certificateStatusChangeResponseHandler(response: CreateApplicationResponse) {
        switch response.status {
        case .pendingPayment:
            applicationPayment = response.payment
            showPaymentView = true
        case .paid:
            completeApplicationStatusChange(applicationId: response.id)
        default:
            showSuccess = true
            successText = formState.successMessage.localized()
        }
    }
}


// MARK: - Enums
extension CreateApplicationViewModel {
    enum FormState {
        case edit
        case preview
        case stopCertificate
        case resumeCertificate
        case revokeCertificate
        
        var isCertificateState: Bool {
            self == .stopCertificate || self == .resumeCertificate || self == .revokeCertificate
        }
        
        var shouldSignCertificateChange: Bool {
            (self == .resumeCertificate || self == .revokeCertificate) && UserManager.getUser()?.acr == .low
        }
        
        var newEIDApplicationType: EIDApplicationType {
            switch self {
            case .stopCertificate:
                return .stop
            case .resumeCertificate:
                return .resume
            case .revokeCertificate:
                return .revoke
            default:
                return .issue
            }
        }
        
        var successMessage: String {
            newEIDApplicationType.successMessage
        }
    }
}

struct CertificateDetailsActionModel {
    var eidAdministratorOfficeId: String = ""
    var eidAdministratorId: String = ""
    var deviceId: String = ""
    var certificateId: String = ""
}

extension CreateApplicationViewModel {
    // MARK: - Fields
    enum Field: Int, CaseIterable {
        case firstNameLatin
        case secondNameLatin
        case lastNameLatin
        case citizenship
        case citizenIdentifier
        case citizenIdentifierType
        case dateOfBirth
        case identityNumber
        case identityIssueDate
        case identityValidityToDate
        case selectedAdministrator
        case selectedDevice
        case selectedOffice
    }
    
    func inputFieldValue(field: Field) -> Validation {
        switch field {
        case .firstNameLatin:
            return firstNameLatin
        case .secondNameLatin:
            return secondNameLatin
        case .lastNameLatin:
            return lastNameLatin
        case .citizenship:
            return citizenship
        case .citizenIdentifier:
            return citizenIdentifier
        case .citizenIdentifierType:
            return citizenIdentifier
        case .dateOfBirth:
            return dateOfBirth
        case .identityNumber:
            return identityNumber
        case .identityIssueDate:
            return identityIssueDate
        case .identityValidityToDate:
            return identityValidityToDate
        case .selectedAdministrator:
            return selectedAdministrator
        case .selectedDevice:
            return selectedDevice
        case .selectedOffice:
            return selectedOffice
        }
    }
}
