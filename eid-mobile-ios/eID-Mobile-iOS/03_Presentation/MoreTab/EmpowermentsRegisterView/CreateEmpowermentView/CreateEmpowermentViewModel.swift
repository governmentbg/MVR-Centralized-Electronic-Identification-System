//
//  CreateEmpowermentViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 31.10.23.
//

import Foundation


final class CreateEmpowermentViewModel: ObservableObject, APICallerViewModel, FieldValidation {
    // MARK: - Properties
    @Published var empowerment: Empowerment?
    @Published var formState: FormState = .edit
    /// Flags
    @Published var showLoading: Bool = false
    @Published var showError: Bool = false
    @Published var errorText: String = ""
    @Published var showSuccess: Bool = false
    @Published var successText: String = ""
    @Published var showInfo: Bool = false
    @Published var isExpanded: Bool = false
    @Published var showProvidersList: Bool = false
    @Published var showServicesList: Bool = false
    @Published var showScopeList: Bool = false
    @Published var showStartDatePicker: Bool = false
    @Published var showEndDatePicker: Bool = false
    @Published var showAddAuthorizerIdButton: Bool = true
    @Published var showEmpowermentField: Bool = false
    @Published var infoText: String = "empowerment_info_description".localized()
    @Published var shouldValidateForm: Bool = false
    /// Input fields
    @Published var authorizerTypeText: String = ""
    @Published var authorizerIdType: IdentifierType = .egn {
        didSet {
            authorizerIdTypeText = authorizerIdType.title.localized()
        }
    }
    @Published var authorizerIdTypeText: String = ""
    @Published var authorizerId: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var authorizerRole: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var authorizerBulstat: BulstatEIK = BulstatEIK(value: "")
    @Published var authorizerName: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var authorizerIds: [PersonalID] = []
    @Published var empoweredIds: [PersonalID] = []
    @Published var empowermentTypeStr: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var providerName: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var serviceName: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var scopeName: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var authorityVolume: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var startDate: Date = .now
    @Published var startDateStr: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    @Published var endDate: Date = .now
    @Published var endDateStr: ValidInput = ValidInput(value: "", rules: [NotEmptyRule()])
    /// API data
    private var providers: [ProviderResponse] = []
    private var services: [ServiceResponse] = []
    private var scope: [ServiceScope] = [] // Sent to server as is
    /// Display data
    @Published var providerItems: [PickerViewItem] = []
    @Published var serviceItems: [PickerViewItem] = []
    @Published var scopeItems: [MultiPickerViewItem] = []
    /// Data to send
    @Published var authorizerType: EmpowermentOnBehalfOf? = nil
    @Published var empowermentType: EmpowermentType? = nil
    @Published var providerId: String = "" {
        didSet {
            providerName.value = providerItems.first(where: { $0.id == providerId })?.name ?? ""
            serviceId = ""
            services = []
            setServiceItems()
            if !providerId.isEmpty {
                getServices()
            }
        }
    }
    @Published var serviceId: String = "" {
        didSet {
            serviceName.value = serviceItems.first(where: { $0.id == serviceId })?.name ?? ""
            allScopeSelected = false
            if serviceId.isEmpty {
                scope = []
                setScopeItems()
            } else {
                getScope()
            }
        }
    }
    @Published var allScopeSelected: Bool = false
    /// Authorizer IDs
    private var validAuthorizerIds: [EmpowermentAuthorizer] {
        var ids = authorizerIds.filter({ !$0.isNotValidField }).map({ EmpowermentAuthorizer(uid: $0.id, uidType: $0.idType, name: $0.name.value, isIssuer: false) })
        if let issuer = ids.filter({ $0.uid == UserManager.getUser()?.citizenIdentifier }).first {
            let index = ids.firstIndex(of: issuer) ?? 0
            ids[index].isIssuer = true
        }
        return ids
    }
    private var hasInvalidAuthorizerIds: Bool {
        return authorizerIds.isEmpty ? true : authorizerIds.filter({ $0.isNotValidField }).first != nil
    }
    /// Empowered IDs
    private var validEmpoweredIds: [UserIdentifier] {
        return empoweredIds.filter({ !$0.isNotValidField }).map({ UserIdentifier(uid: $0.id, uidType: $0.idType, name: $0.name.value) })
    }
    private var hasInvalidEmpoweredIds: Bool {
        return empoweredIds.isEmpty ? true : empoweredIds.filter({ $0.isNotValidField }).first != nil
    }
    /// Computed properties
    private var selectedScopes: [ServiceScope] {
        return scope.filter({ scopeItems.filter({ $0.selected }).map({ $0.id }).contains($0.id) })
    }
    private var hasEndDate: Bool = false
    /// Indefinite alert properties
    @Published var showIndefiniteAlert: Bool = false
    
    // MARK: - Init
    init(empowement: Empowerment? = nil) {
        self.empowerment = empowement
        if let empowement = empowement {
            setAuthorizerType(empowement.onBehalfOf)
            if empowement.onBehalfOf == .legalEntity {
                authorizerRole.value = empowement.issuerPosition ?? ""
                authorizerBulstat.value = empowement.uid ?? ""
                authorizerName.value = empowement.name ?? ""
            }
        }
    }
    
    // MARK: - Public
    func setAuthorizerType(_ newType: EmpowermentOnBehalfOf) {
        guard newType != authorizerType else { return }
        isExpanded = true
        authorizerType = newType
        authorizerTypeText = newType.title.localized()
        if newType == .individual {
            let user = UserManager.getUser()
            authorizerIdType = IdentifierType(rawValue: user?.citizenIdentifierType ?? "") ?? .egn
            authorizerId.value = user?.citizenIdentifier ?? ""
        } else if newType == .legalEntity {
            initAuthorizerIds()
        }
        clearLegalEntityData()
        providerId = ""
        getProviders()
        initEmpoweredIds()
        setStartDate()
        infoText = newType.info.localized()
    }
    
    func setEmpowermentType(_ newType: EmpowermentType?) {
        empowermentType = newType
        empowermentTypeStr.value = newType?.title.localized() ?? ""
    }
    
    // MARK: - API calls
    private func getProviders() {
        let request = GetProvidersRequest(pageIndex: 1)
        showLoading = true
        EmpowermentsRegisterRouter.getProviders(input: request)
            .send(GetProvidersPageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let providersPage):
                    guard let providersPage = providersPage else { return }
                    self?.providers.appendIfMissing(contentsOf: providersPage.data)
                    self?.setProviderItems()
                    self?.setProviderIfNeeded()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func getServices() {
        guard !providerId.isEmpty else { return }
        let request = GetServicesRequest(providerid: providerId,
                                         pageIndex: 1)
        showLoading = true
        EmpowermentsRegisterRouter.getServices(input: request)
            .send(GetServicesPageResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let servicesPage):
                    guard let servicesPage = servicesPage else { return }
                    self?.services.appendIfMissing(contentsOf: servicesPage.data)
                    self?.setServiceItems()
                    self?.setServiceIfNeeded()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    private func getScope() {
        guard !serviceId.isEmpty else { return }
        let request = GetServiceScopeRequest(serviceId: serviceId)
        showLoading = true
        EmpowermentsRegisterRouter.getServiceScope(input: request)
            .send(GetServiceScopeResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success(let scopeResponse):
                    guard let scopeResponse = scopeResponse else { return }
                    self?.scope = scopeResponse
                    self?.setScopeItems()
                    self?.setScopeIfNeeded()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    func createEmpowerment() {
        guard let onBehalfOf = authorizerType?.rawValue,
              let serviceNumber = services.first(where: { $0.id == serviceId })?.serviceNumber,
              validateFields()
        else {
            return
        }
        showLoading = true
        
        var authorizersToSubmit: [EmpowermentAuthorizer] = []
        if authorizerType == .legalEntity {
            authorizersToSubmit = validAuthorizerIds
            if let issuer = authorizersToSubmit.filter({ $0.isIssuer == true }).first {
                let index = authorizersToSubmit.firstIndex(of: issuer) ?? 0
                authorizersToSubmit.remove(at: index)
            }
        }
        let request = CreateEmpowermentRequest(onBehalfOf: onBehalfOf,
                                               issuerPosition: authorizerRole.value.isEmpty ? nil : authorizerRole.value,
                                               uid: authorizerBulstat.value.isEmpty ? nil : authorizerBulstat.value,
                                               uidType: .egn,
                                               authorizerUids: authorizerType == .legalEntity ? authorizersToSubmit : [],
                                               name: authorizerName.value.isEmpty ? nil : authorizerName.value,
                                               empoweredUids: validEmpoweredIds,
                                               typeOfEmpowerment: empowermentType?.rawValue ?? 0,
                                               providerId: providerId,
                                               providerName: providerName.value,
                                               serviceId: serviceNumber,
                                               serviceName: serviceName.value,
                                               volumeOfRepresentation: selectedScopes,
                                               startDate: startDate.normalizeDate(outputFormat: .iso8601UTC),
                                               expiryDate: hasEndDate ? endDate.toLocalTime().endOfDay.toGlobalTime().normalizeDate(outputFormat: .iso8601UTC) : nil)
        EmpowermentsRegisterRouter.createEmpowerment(input: request)
            .send(CreateEmpowermentResponse.self) { [weak self] response in
                self?.showLoading = false
                switch response {
                case .success:
                    self?.showSuccess = true
                    self?.successText = "empowerment_success_title".localized()
                case .failure(let error):
                    self?.showError = true
                    self?.errorText = error.localizedDescription
                }
            }
    }
    
    // MARK: - Public Helpers
    func addNewIdField(isAuthorizer: Bool) {
        let id = PersonalID(idType: .egn,
                            idTypeText: IdentifierType.egn.title.localized(),
                            id: "",
                            name: Name(value: "",
                                       isLatin: false,
                                       isMandatory: true),
                            visible: true,
                            shouldValidateEmpowerment: !isAuthorizer)
        if isAuthorizer {
            authorizerIds.append(id)
            showAddAuthorizerIdButton = authorizerIds.count < 10
        } else {
            empoweredIds.append(id)
            setEmpoweredButtonStates()
        }
    }
    
    func setIdFieldType(_ idType: IdentifierType, index: Int, isAuthorizer: Bool) {
        if isAuthorizer {
            guard index < authorizerIds.count else { return }
            authorizerIds[index].id = ""
            authorizerIds[index].idType = idType
        } else {
            guard index < empoweredIds.count else { return }
            empoweredIds[index].id = ""
            empoweredIds[index].idType = idType
        }
    }
    
    func removeIdField(index: Int, isAuthorizer: Bool) {
        if isAuthorizer {
            guard index < authorizerIds.count else { return }
            authorizerIds.remove(at: index)
            showAddAuthorizerIdButton = authorizerIds.count < 10
            validateAuthorizers()
        } else {
            guard index < empoweredIds.count else { return }
            empoweredIds.remove(at: index)
            setEmpoweredButtonStates()
            if empoweredIds.count <= 1 {
                setEmpowermentType(nil)
            }
            validateEmpowerers()
        }
    }
    
    func scopeChanged() {
        var name = ""
        let filteredScopeItems = scopeItems.filter{ $0.selected }
        if !filteredScopeItems.isEmpty {
            for scope in filteredScopeItems {
                name.append("\(scope.name), ")
            }
            name.removeLast(2)
        }
        scopeName.value = name
    }
    
    func setStartDate() {
        startDateStr.value = startDate.toLocalTime().normalizeDate(outputFormat: .iso8601Date)
        clearEndDate()
    }
    
    func setEndDate() {
        endDateStr.value = endDate.toLocalTime().normalizeDate(outputFormat: .iso8601Date)
        hasEndDate = true
    }
    
    func clearEndDate() {
        hasEndDate = false
        endDateStr.value = ""
        endDate = startDate
    }
    
    func clearStartDate() {
        startDateStr.value = ""
        startDate = .now
    }
    
    func changeState(to newState: FormState) {
        formState = newState
    }
    
    func checkEmpowermentValidity() {
        shouldValidateForm = true
        guard validateFields() else { return }
        if endDateStr.validation.isNotValid {
            showIndefiniteAlert.toggle()
        } else {
            changeState(to: .preview)
        }
    }
    
    func validateAuthorizers() {
        let currentIds = authorizerIds.filter({ $0.id.isEmpty == false }).map({ $0.id })
        for i in authorizerIds.indices {
            authorizerIds[i].currentIds = currentIds
        }
    }
    
    func validateEmpowerers() {
        let currentIds = empoweredIds.filter({ $0.id.isEmpty == false }).map({ $0.id })
        for i in empoweredIds.indices {
            empoweredIds[i].currentIds = currentIds
        }
    }
    
    // MARK: - Private Helpers
    private func initAuthorizerIds() {
        var ids: [PersonalID] = []
        let user = UserManager.getUser()
        let userIdType = IdentifierType(rawValue: user?.citizenIdentifierType ?? "") ?? .egn
        let userId = PersonalID(idType: userIdType,
                                idTypeText: userIdType.title.localized(),
                                id: user?.citizenIdentifier ?? "",
                                name: Name(value: user?.nameCyrillic ?? "",
                                           isLatin: false,
                                           isMandatory: true),
                                visible: true)
        ids.append(userId)
        if let empowerment = empowerment,
           empowerment.authorizerUids.count < 10 {
            for authorizer in
                    empowerment.authorizerUids.sorted(by: { (($0.isIssuer ?? false) && !($1.isIssuer ?? false)) }) {
                let id = PersonalID(idType: authorizer.uidType,
                                    idTypeText: authorizer.uidType.title.localized(),
                                    id: authorizer.uid,
                                    name: Name(value: authorizer.name ?? "",
                                               isLatin: false,
                                               isMandatory: true),
                                    visible: true)
                ids.append(id)
            }
        }
        authorizerIds = ids
        showAddAuthorizerIdButton = authorizerIds.count < 10
    }
    
    private func initEmpoweredIds() {
        var ids: [PersonalID] = []
        if let empowerment = empowerment,
           empowerment.empoweredUids.count < 10 {
            for empoweredUid in empowerment.empoweredUids {
                let id = PersonalID(idType: empoweredUid.uidType ?? .egn,
                                    idTypeText: empoweredUid.uidType?.title.localized() ?? "",
                                    id: empoweredUid.uid ?? "",
                                    name: Name(value: empoweredUid.name ?? "",
                                               isLatin: false,
                                               isMandatory: true),
                                    visible: true)
                ids.append(id)
            }
            if empowerment.empoweredUids.count > 1 {
                setEmpowermentType(.togetherOnly)
            }
        }
        empoweredIds = ids
        if empoweredIds.isEmpty {
            addNewIdField(isAuthorizer: false)
        }
        setEmpoweredButtonStates()
    }
    
    private func setEmpoweredButtonStates() {
        showEmpowermentField = empoweredIds.count > 1
    }
    
    private func setProviderItems() {
        providerItems = providers.map { PickerViewItem(id: $0.id, name: $0.name ?? "") }
    }
    
    private func setProviderIfNeeded() {
        guard let empowerment = empowerment,
              let suppier = providerItems.first(where: { $0.name == empowerment.providerName })
        else { return }
        providerId = suppier.id
    }
    
    private func setServiceItems() {
        serviceItems = services.map { PickerViewItem(id: $0.id ?? "", name: $0.name ?? "") }
    }
    
    private func setServiceIfNeeded() {
        guard let empowerment = empowerment,
              let service = services.first(where: { $0.serviceNumber == empowerment.serviceId })
        else { return }
        serviceId = service.id ?? ""
    }
    
    private func setScopeItems() {
        scopeItems = scope.map { MultiPickerViewItem(id: $0.id, name: $0.name, selected: false) }
        scopeChanged()
    }
    
    private func setScopeIfNeeded() {
        guard let empowerment = empowerment else { return }
        let empowermentVolumeNames = empowerment.volumeOfRepresentation.map { $0.name }
        scopeItems = scope.map { MultiPickerViewItem(id: $0.id, name: $0.name, selected: empowermentVolumeNames.contains($0.name)) }
        scopeChanged()
    }
    
    private func clearLegalEntityData() {
        authorizerRole.value = ""
        authorizerBulstat.value = ""
        authorizerName.value = ""
    }
    
    @discardableResult func validateFields() -> Bool {
        let isValid = authorizerType != nil
        && !hasInvalidEmpoweredIds
        && providerName.validation.isValid
        && serviceName.validation.isValid
        && !selectedScopes.isEmpty
        && startDateStr.validation.isValid
        switch authorizerType {
        case .individual:
            return isValid && authorizerId.validation.isValid
        case .legalEntity:
            let legalEntityInputIsValid = authorizerRole.validation.isValid
            && authorizerBulstat.validation.isValid
            && authorizerName.validation.isValid
            && !hasInvalidAuthorizerIds
            return isValid && legalEntityInputIsValid
        default:
            return false
        }
    }
}


// MARK: - Enums
extension CreateEmpowermentViewModel {
    enum FormState {
        case edit
        case preview
    }
    
    // MARK: - Fields
    enum Field: Int, CaseIterable {
        case authorizerId
        case authorizerRole
        case authorizerBulstat
        case authorizerName
        case authorizerIds
        case empoweredIds
        case empowermentType
        case selectedProvider
        case selectedService
        case selectedScope
        case startDate
    }
    
    private var allFieldsForErrorValidation: [Field] {
        return authorizerType == .legalEntity
        ? [.authorizerRole,
           .authorizerBulstat,
           .authorizerName,
           .authorizerIds,
           .empoweredIds,
           .selectedProvider,
           .selectedService,
           .selectedScope,
           .startDate]
        
        : [.authorizerId,
           .empoweredIds,
           .selectedProvider,
           .selectedService,
           .selectedScope,
           .startDate]
    }
    
    var firstErrorField: Field? {
        var firstErrorField: Field?
        for field in allFieldsForErrorValidation {
            let value = inputFieldValue(field: field)
            if let personalId = value as? PersonalID {
                if personalId.id.isEmpty
                    || personalId.validation.isNotValid
                    || personalId.name.validation.isNotValid {
                    firstErrorField = field
                    break
                }
            } else {
                if value.validation.isNotValid {
                    firstErrorField = field
                    break
                }
            }
        }
        return firstErrorField
    }
    
    func inputFieldValue(field: Field) -> Validation {
        switch field {
        case .authorizerId:
            return authorizerId
        case .empowermentType:
            return empowermentTypeStr
        case .selectedProvider:
            return providerName
        case .selectedService:
            return serviceName
        case .selectedScope:
            return scopeName
        case .startDate:
            return startDateStr
        case .authorizerIds:
            let invalidAuthorizer: Validation = authorizerIds.filter({ $0.isNotValidField }).first ?? ValidInput(value: "", rules: [])
            return authorizerIds.isEmpty ? ValidInput(value: "", rules: [NotEmptyRule()]) : invalidAuthorizer
            
        case .empoweredIds:
            let invalidEmpowered: Validation = empoweredIds.filter({ $0.isNotValidField }).first ?? ValidInput(value: "", rules: [])
            return empoweredIds.isEmpty ? ValidInput(value: "", rules: [NotEmptyRule()]) : invalidEmpowered
            
        case .authorizerRole:
            return authorizerRole
        case .authorizerBulstat:
            return authorizerBulstat
        case .authorizerName:
            return authorizerName
        }
    }
}


// MARK: - Public Enums
enum EmpowermentType: Int {
    case separately = 0
    case togetherOnly = 1
    
    var title: String {
        switch self {
        case .separately:
            return "empowerment_type_separately"
        case .togetherOnly:
            return "empowerment_type_together_only"
        }
    }
}
