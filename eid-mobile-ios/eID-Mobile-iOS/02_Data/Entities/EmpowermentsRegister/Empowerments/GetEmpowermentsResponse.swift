//
//  GetEmpowermentsResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.11.23.
//

import Foundation


// MARK: - Helper Enums
enum EmpowermentsDenialReason: String, Codable {
    case none = "None"
    case deceasedUid = "DeceasedUid"
    case prohibitedUid = "ProhibitedUid"
    case NTRCheckFailed = "NTRCheckFailed"
    case timedOut = "TimedOut"
    case belowLawfulAge = "BelowLawfulAge"
    case noPermit = "NoPermit"
    case lawfulAgeInfoNotAvailable = "LawfulAgeInfoNotAvailable"
    case unsuccessfulRestrictionsCheck = "UnsuccessfulRestrictionsCheck"
    case legalEntityNotActive = "LegalEntityNotActive"
    case legalEntityRepresentationNotMatch = "LegalEntityRepresentationNotMatch"
    case unsuccessfulLegalEntityCheck = "UnsuccessfulLegalEntityCheck"
    case empowermentStatementNotFound = "EmpowermentStatementNotFound"
    case signatureCollectionTimeOut = "SignatureCollectionTimeOut"
    case inactiveInBulstat = "InactiveInBulstat"
    case closedInBulstat = "ClosedInBulstat"
    case archivedInBulstat = "ArchivedInBulstat"
    case bulstatCheckFailed = "BulstatCheckFailed"
    case reregisteredInNTR = "ReregisteredInNTR"
    case inInsolvencyProceedingsInBulstat = "InInsolvencyProceedingsInBulstat"
    case insolventInBulstat = "InsolventInBulstat"
    case inLiquidationInBulstat = "InLiquidationInBulstat"
    case unsuccessfulTimestamping = "UnsuccessfulTimestamping"
    case invalidUidRegistrationStatusDetected = "InvalidUidRegistrationStatusDetected"
    case deniedByDeauAdministrator = "DeniedByDeauAdministrator"
    case uidsRegistrationStatusInfoNotAvailable = "UidsRegistrationStatusInfoNotAvailable"
    case registrationStatusUnavailable = "RegistrationStatusUnavailable"
    case inactiveProfile = "InactiveProfile"
    case noBaseProfile = "NoBaseProfile"
    case nameMismatch = "NameMismatch"
    case noRegistration = "NoRegistration"
    case unknown = ""
}

extension EmpowermentsDenialReason {
    init(from decoder: any Decoder) throws {
        let container = try decoder.singleValueContainer()
        let rawValue = try container.decode(String.self)
        self = .init(rawValue: rawValue) ?? .unknown
    }
}

extension EmpowermentsDenialReason {
    var description: String {
        switch self {
        case .none:
            return "empowerments_denial_reason_none"
        case .deceasedUid:
            return "empowerments_denial_reason_deceased_uid"
        case .prohibitedUid:
            return "empowerments_denial_reason_prohibited_uid"
        case .NTRCheckFailed:
            return "empowerments_denial_reason_ntr_check_failed"
        case .timedOut:
            return "empowerments_denial_reason_timed_out"
        case .belowLawfulAge:
            return "empowerments_denial_reason_below_lawful_age"
        case .noPermit:
            return "empowerments_denial_reason_no_permit"
        case .lawfulAgeInfoNotAvailable:
            return "empowerments_denial_reason_lawful_age_info_not_available"
        case .unsuccessfulRestrictionsCheck:
            return "empowerments_denial_reason_unsuccessful_restrictions_check"
        case .legalEntityNotActive:
            return "empowerments_denial_reason_legal_entity_not_active"
        case .legalEntityRepresentationNotMatch:
            return "empowerments_denial_reason_legal_entity_representation_not_match"
        case .unsuccessfulLegalEntityCheck:
            return "empowerments_denial_reason_unsuccessful_legal_entity_check"
        case .empowermentStatementNotFound:
            return "empowerments_denial_reason_empowerment_statement_not_found"
        case .signatureCollectionTimeOut:
            return "empowerments_denial_reason_signature_collection_time_out"
        case .inactiveInBulstat:
            return "empowerments_denial_reason_inactive_in_bulstat"
        case .closedInBulstat:
            return "empowerments_denial_reason_closed_in_bulstat"
        case .archivedInBulstat:
            return "empowerments_denial_reason_archived_in_bulstat"
        case .bulstatCheckFailed:
            return "empowerments_denial_reason_bulstat_check_failed"
        case .reregisteredInNTR:
            return "empowerments_denial_reason_reregistered_in_NTR"
        case .inInsolvencyProceedingsInBulstat:
            return "empowerments_denial_reason_in_insolvency_proceedings_in_bulstat"
        case .insolventInBulstat:
            return "empowerments_denial_reason_insolvent_in_bulstat"
        case .inLiquidationInBulstat:
            return "empowerments_denial_reason_in_liquidation_in_bulstat"
        case .unsuccessfulTimestamping:
            return "empowerments_denial_reason_unsuccessful_timestamping"
        case .invalidUidRegistrationStatusDetected:
            return "empowerments_denial_reason_invalid_uid_registration_status_detected"
        case .deniedByDeauAdministrator:
            return "empowerments_denial_reason_denied_by_deau_administrator"
        case .uidsRegistrationStatusInfoNotAvailable:
            return "empowerments_denial_reason_uids_registration_status_info_not_available"
        case .registrationStatusUnavailable:
            return "empowerments_denial_reason_registration_status_unavailable"
        case .inactiveProfile:
            return "empowerments_denial_reason_inactive_profile"
        case .noBaseProfile, .noRegistration:
            return "empowerments_denial_reason_no_base_profile"
        case .nameMismatch:
            return "empowerments_denial_reason_name_mismatch"
        case .unknown:
            return "status_unknown"
        }
    }
}

enum EmpowermentWithdrawalStatus: String, Codable {
    case none = "None"
    case inProgress = "InProgress"
    case completed = "Completed"
    case denied = "Denied"
    case timeout = "Timeout"
}


// MARK: - GetEmpowermentsPageResponse
struct GetEmpowermentsPageResponse: Codable {
    var pageIndex: Int
    var totalItems: Int
    var data: [Empowerment]
}


// MARK: - Empowerment
struct Empowerment: Codable, Hashable {
    // MARK: - Properties
    var empowermentSignatures: [EmpowermentSignatureResponse]?
    var xmlRepresentation: String?
    var id: String?
    var number: String?
    var startDate: String?
    var expiryDate: String?
    var status: EmpowermentStatus?
    var calculatedStatusOn: EmpowermentStatus?
    var uid: String?
    var uidType: IdentifierType?
    var name: String?
    var onBehalfOf: EmpowermentOnBehalfOf
    var authorizerUids: [EmpowermentAuthorizer]
    var empoweredUids: [UserIdentifier]
    var providerId: String?
    var providerName: String?
    var serviceId: Int?
    var serviceName: String?
    var volumeOfRepresentation: [EmpowermentVolumeOfRepresentation]
    var createdOn: String?
    var createdBy: String?
    var issuerPosition: String?
    var denialReason: EmpowermentsDenialReason?
    var empowermentWithdrawals: [EmpowermentWithdrawal]
    var empowermentDisagreements: [EmpowermentDisagreement]
    private var statusHistory: [EmpowermentStatusHistory]
    
    // MARK: - Hashable
    static func == (lhs: Empowerment, rhs: Empowerment) -> Bool {
        return lhs.id == rhs.id
    }
}

struct EmpowermentSignatureResponse: Codable, Hashable {
    // MARK: - Properties
    let dateTime: String?
    let signerUid: String?
    let signature: String?
}

struct EmpowermentVolumeOfRepresentation: Codable, Hashable {
    // MARK: - Properties
    var code: String?
    var name: String?
}

struct EmpowermentWithdrawal: Codable, Hashable {
    // MARK: - Properties
    var startDateTime: String?
    var activeDateTime: String?
    var issuerUid: String?
    var reason: String?
    var status: EmpowermentWithdrawalStatus
}

struct EmpowermentDisagreement: Codable, Hashable {
    // MARK: - Properties
    var activeDateTime: String?
    var issuerUid: String?
    var issuerUidType: IdentifierType?
    var reason: String?
}

struct EmpowermentStatusHistory: Codable, Hashable {
    // MARK: - Properties
    var id: String?
    var dateTime: String?
    var status: EmpowermentStatus?
}

// MARK: - Helpers
extension Empowerment {
    var volumeOfRepresentationDescription: String {
        var description: String = ""
        for volumeOfRepresentation in self.volumeOfRepresentation {
            description.append("\(volumeOfRepresentation.name ?? ""), ")
        }
        description.removeLast(2)
        return description
    }
    
    var withdrawalReason: String? {
        return empowermentWithdrawals.filter({ $0.status == .completed }).first?.reason
    }
    
    var disagreementReason: String? {
        let reasons = empowermentDisagreements.filter({ $0.reason?.isEmpty == false }).map({ $0.reason ?? "" })
        return reasons.joined(separator: ",\n")
    }
    
    var statusReason: String? {
        switch status {
        case .withdrawn:
            guard let reason = withdrawalReason else { return nil }
            return String(format: "empowerment_status_reason_title".localized(), reason)
        case .disagreementDeclared:
            guard let reason = disagreementReason else { return nil }
            return String(format: "empowerment_status_reason_title".localized(), reason)
        case .denied:
            return String(format: "empowerment_status_reason_title".localized(), denialReason?.description.localized() ?? "")
        default:
            return nil
        }
    }
    
    var history: [EmpowermentStatusHistory] {
        return statusHistory.filter({ $0.status != .collectingWithdrawalSignatures && $0.status != .awaitingSignature })
    }
    
    var shouldSign: Bool {
        return empowermentSignatures?.filter({ $0.signerUid == (UserManager.getUser()?.citizenIdentifier ?? "") }).first == nil
    }
}
