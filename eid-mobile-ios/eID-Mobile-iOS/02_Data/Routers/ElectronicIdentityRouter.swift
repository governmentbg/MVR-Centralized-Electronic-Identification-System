//
//  ElectronicIdentityRouter.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.02.24.
//

import Foundation
import Alamofire

enum ElectronicIdentityRouter: URLRequestBuilder {
    // MARK: - Endpoints
    /// New application
    case generateApplicationXml(input: GenerateApplicationXmlRequest)
    case createApplication(input: CreateApplicationRequest)
    /// Applications
    case getApplications(input: ApplicationsRequest)
    case getApplicationDetails(input: ApplicationDetailsRequest)
    /// Certificates
    case getCertificates(input: CertificatesRequest)
    case getCertificateDetails(input: CertificateDetailsRequest)
    case getCertificateHistory(input: CertificateDetailsRequest)
    case setCertificateAlias(input: SetCertificateAliasRequest)
    case changeCertificateStatusPlain(input: ChangeCertificateStatusRequest)
    case changeCertificateStatusSigned(input: CreateApplicationRequest)
    case completeCertificateStatusChange(input: CompleteCertificateStatusChangeRequest)
    /// Complete application flow - base profile
    case signApplicationBaseProfile(input: SignApplicationRequest)
    case enrollCertificateBaseProfile(input: EnrollCertificateBaseProfileRequest)
    case confirmCertificateStorageBaseProfile(input: ConfirmCertificateStorageBaseProfileRequest)
    /// Complete application flow - EID
    case enrollCertificateEID(input: EnrollCertificateEIDRequest)
    case confirmCertificateStorageEID(input: ConfirmCertificateStorageEIDRequest)
    /// Enums
    case getAdministrators
    case getAdministratorById(input: GetDataById)
    case getDevices
    case getDeviceById(input: GetDataById)
    case getOfficeByAdministratorId(input: GetDataById)
    case getOfficeById(input: GetDataById)
    case getCertificateActionReasons
    /// Citizen EID
    case getCitizenEID
    /// Log event
    case logEvent(input: LogEventRequest)
    
    // MARK: - BaseURL
    var mainURL: URL  {
        switch self {
        case .getAdministrators,
                .getAdministratorById,
                .getDevices,
                .getDeviceById,
                .getOfficeByAdministratorId,
                .getOfficeById:
            return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.RAEICEI.baseUrl) as! String)!
        default:
            return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.MPOZEI.baseUrl) as! String)!
        }
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
            /// New application
        case .generateApplicationXml:
            return "mpozei/external/api/v1/applications/generate-xml"
        case .createApplication:
            return "mpozei/external/api/v1/applications"
            /// Applications
        case .getApplications(let input):
            return "mpozei/external/api/v1/applications/find\(input.queryParams())"
        case .getApplicationDetails(let input):
            return "mpozei/external/api/v1/applications/\(input.id)"
            /// Certificates
        case .changeCertificateStatusPlain(_):
            return "mpozei/external/api/v1/applications/certificate-status-change/plain"
        case .changeCertificateStatusSigned(_):
            return "mpozei/external/api/v1/applications/certificate-status-change/signed"
        case .completeCertificateStatusChange(input: let input):
            return "mpozei/external/api/v1/applications/\(input.applicationId)/complete"
        case .getCertificates(let input):
            return "mpozei/external/api/v1/certificates/find\(input.queryParams())"
        case .getCertificateDetails(let input):
            return "mpozei/external/api/v1/certificates/\(input.id)"
        case .getCertificateHistory(let input):
            return "mpozei/external/api/v1/certificates/\(input.id)/history"
        case .signApplicationBaseProfile:
            return "mpozei/external/api/v1/applications/sign"
        case .enrollCertificateBaseProfile:
            return "mpozei/external/api/v1/mobile/certificates/enroll/base-profile"
        case .confirmCertificateStorageBaseProfile:
            return "mpozei/external/api/v1/mobile/certificates/confirm"
        case .enrollCertificateEID:
            return "mpozei/external/api/v1/mobile/certificates/enroll/eid"
        case .confirmCertificateStorageEID:
            return "mpozei/external/api/v1/mobile/certificates/confirm/eid"
            /// Enums
        case .getAdministrators:
            return "raeicei/external/api/v1/eidadministrator/getAll"
        case .getAdministratorById(let input):
            return "raeicei/external/api/v1/eidadministrator/\(input.id)"
        case .getDevices:
            return "raeicei/external/api/v1/device/getAll"
        case .getDeviceById(let input):
            return "raeicei/external/api/v1/device/\(input.id)"
        case .getOfficeByAdministratorId(let input):
            return "raeicei/external/api/v1/eidmanagerfrontoffice/getAll/\(input.id)"
        case .getOfficeById(let input):
            return "raeicei/external/api/v1/eidmanagerfrontoffice/\(input.id)"
        case .getCertificateActionReasons:
            return "mpozei/external/api/v1/nomenclatures/reasons"
            /// Citizen EID
        case .getCitizenEID:
            return "mpozei/external/api/v1/eidentities"
        case .logEvent:
            return "mpozei/external/api/v1/device/log"
        case .setCertificateAlias(let input):
            return "mpozei/external/api/v1/certificates/alias\(input.queryParams.queryParams())"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .getApplications,
                .getApplicationDetails,
                .getCertificates,
                .getCertificateDetails,
                .getCertificateHistory,
                .getAdministrators,
                .getAdministratorById,
                .getDevices,
                .getDeviceById,
                .getOfficeByAdministratorId,
                .getOfficeById,
                .getCertificateActionReasons,
                .getCitizenEID:
            return .get
        case .generateApplicationXml,
                .createApplication,
                .changeCertificateStatusPlain(_),
                .changeCertificateStatusSigned(_),
                .completeCertificateStatusChange,
                .signApplicationBaseProfile,
                .enrollCertificateBaseProfile,
                .confirmCertificateStorageBaseProfile,
                .enrollCertificateEID,
                .confirmCertificateStorageEID,
                .logEvent:
            return .post
        case .setCertificateAlias:
            return .put
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .generateApplicationXml(let input):
            params.append(input.toDict())
        case .createApplication(let input):
            params.append(input.toDict())
        case .changeCertificateStatusPlain(input: let input):
            params.append(input.toDict())
        case .changeCertificateStatusSigned(input: let input):
            params.append(input.toDict())
        case .signApplicationBaseProfile(let input):
            params.append(input.toDict())
        case .enrollCertificateBaseProfile(let input):
            params.append(input.toDict())
        case .confirmCertificateStorageBaseProfile(let input):
            params.append(input.toDict())
        case .enrollCertificateEID(let input):
            params.append(input.toDict())
        case .confirmCertificateStorageEID(let input):
            params.append(input.toDict())
        case .logEvent(let input):
            params.append(input.toDict())
        case .setCertificateAlias(let input):
            params.append(input.bodyParams.toDict())
        default:
            break
        }
        return params
    }
    
    // MARK: - Headers
    var headers: HTTPHeaders?  {
        var headers: HTTPHeaders = []
        headers.add(HTTPHeader.contentType("application/json"))
        headers.add(HTTPHeader(name: "Cookie", value: "KEYCLOAK_LOCALE=bg"))
        if let bearerToken = StorageManager.keychain.getFor(key: .authToken) {
            headers.add(HTTPHeader.authorization(bearerToken: bearerToken))
        }
        return headers
    }
    
    var requestURL: URL {
        switch self {
        case .setCertificateAlias,
                .getApplications,
                .getCertificates:
            if let fullPath = URL(string: mainURL.appendingPathComponent(path).absoluteString.removingPercentEncoding ?? "") {
                return fullPath
            }
        default: break
        }
        
        return mainURL.appendingPathComponent(path)
    }
}
