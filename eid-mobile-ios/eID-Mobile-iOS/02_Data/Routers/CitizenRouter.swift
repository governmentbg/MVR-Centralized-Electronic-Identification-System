//
//  CitizenRouter.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 14.05.24.
//

import Foundation
import Alamofire


enum CitizenRouter: URLRequestBuilder {
    case register(input: RegisterCitizenRequest)
    case changeEmail(input: ChangeCitizenEmailRequest)
    case changePassword(input: ChangeCitizenPasswordRequest)
    case forgottenPasswordCheck(input: ForgottenPasswordCheckRequest)
    case changeInformation(input: ChangeCitizenInformationRequest)
    case associateEid(input: CitizenAssociateEidRequest)
    
    var mainURL: URL  {
        switch self {
        case .associateEid:
            return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.ISCEI.baseUrl) as! String)!
        default:
            return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.PG.baseUrl) as! String)!
        }
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .register:
            return "mpozei/external/api/v1/citizens/register"
        case .changeEmail(let input):
            return "mpozei/external/api/v1/citizens/update-email\(input.queryParams())"
        case .changePassword(let input):
            return "mpozei/external/api/v1/citizens/update-password\(input.queryParams())"
        case .forgottenPasswordCheck:
            return "mpozei/external/api/v1/citizens/forgotten-password"
        case .changeInformation:
            return "mpozei/external/api/v1/citizens"
        case .associateEid(let input):
            return "iscei/api/v1/auth/associate-profiles\(input.queryParams.queryParams())"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .register,
                .changeEmail,
                .changePassword,
                .forgottenPasswordCheck,
                .associateEid:
            return .post
        case .changeInformation:
            return .put
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .register(let input):
            params.append(input.toDict())
        case .forgottenPasswordCheck(let input):
            params.append(input.toDict())
        case .associateEid(let input):
            params.append(input.bodyParams.toDict())
        case .changeEmail:
            break
        case .changePassword:
            break
        case .changeInformation(input: let input):
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
    
    var requestURL: URL {
        switch self {
        case .changeEmail, .changePassword, .associateEid:
            if let fullPath = URL(string: mainURL.appendingPathComponent(path).absoluteString.removingPercentEncoding ?? "") {
                return fullPath
            }
        default: break
        }
        return  mainURL.appendingPathComponent(path)
    }
}
