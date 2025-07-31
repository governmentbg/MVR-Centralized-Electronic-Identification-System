//
//  EmpowermentsRegistryRouter.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.11.23.
//

import Foundation
import Alamofire


enum EmpowermentsRegisterRouter: URLRequestBuilder {
    // MARK: - Endpoints
    case getProviders(input: GetProvidersRequest)
    case getServices(input: GetServicesRequest)
    case getServiceScope(input: GetServiceScopeRequest)
    case createEmpowerment(input: CreateEmpowermentRequest)
    case getEmpowermentsFromMe(input: GetEmpowermentsRequest)
    case getEmpowermentsToMe(input: GetEmpowermentsRequest)
    case getEmpowermentsFromMeEIK(input: GetEmpowermentsRequest)
    case withdrawEmpowerment(input: EmpowermentActionRequest)
    case declareDisagreement(input: EmpowermentActionRequest)
    case signEmpowerment(input: EmpowermentSignRequest)
    
    // MARK: - BaseURL
    var mainURL: URL  {
        return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.PG.baseUrl) as! String)!
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .getProviders:
            return "ro/api/v1/providers"
        case .getServices:
            return "ro/api/v1/providers/services"
        case .getServiceScope(let input):
            return "ro/api/v1/providers/services/\(input.serviceId)/scope"
        case .createEmpowerment:
            return "ro/api/v1/empowerments"
        case .getEmpowermentsFromMe:
            return "ro/api/v1/empowerments/from"
        case .getEmpowermentsToMe:
            return "ro/api/v1/empowerments/to"
        case .getEmpowermentsFromMeEIK:
            return "ro/api/v1/empowerments/eik"
        case .withdrawEmpowerment(let input):
            return "ro/api/v1/Empowerments/\(input.empowermentId)/withdraw"
        case .declareDisagreement(let input):
            return "ro/api/v1/Empowerments/\(input.empowermentId)/disagreement"
        case .signEmpowerment(let input):
            return "ro/api/v1/Empowerments/\(input.empowermentId)/sign"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .getProviders,
                .getServices,
                .getServiceScope,
                .getEmpowermentsToMe:
            return .get
        case .createEmpowerment,
                .getEmpowermentsFromMe,
                .withdrawEmpowerment,
                .declareDisagreement,
                .signEmpowerment,
                .getEmpowermentsFromMeEIK:
            return .post
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .getProviders(let input):
            params.append(input.toDict())
        case .getServices(let input):
            params.append(input.toDict())
        case .getServiceScope(let input):
            params.append(input.toDict())
        case .createEmpowerment(let input):
            params.append(input.toDict())
        case .getEmpowermentsFromMe(let input):
            params.append(input.toDict())
        case .getEmpowermentsToMe(let input):
            params.append(input.toDict())
        case .getEmpowermentsFromMeEIK(let input):
            params.append(input.toDict())
        case .withdrawEmpowerment(let input):
            params.append(input.toDict())
        case .declareDisagreement(let input):
            params.append(input.toDict())
        case .signEmpowerment(let input):
            params.append(input.toDict())
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
}
