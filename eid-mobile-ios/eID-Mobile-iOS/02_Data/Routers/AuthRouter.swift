//
//  AuthRouter.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 4.07.24.
//

import Foundation
import Alamofire


enum AuthRouter: URLRequestBuilder {
    // MARK: - Endpoints
    case basicLogin(input: BasicLoginRequest)
    case getChallenge(input: GetChallengeRequest)
    case certificateLogin(input: CertificateLoginRequest)
    case verifyLogin(input: VerifyLoginRequest)
    case generateOTPCode(input: GenerateOTPCodeRequest)
    case verifyOTPCode(input: VerifyOTPCodeRequest)
    
    // MARK: - BaseURL
    var mainURL: URL  {
        switch self {
        case .verifyLogin:
            return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.MPOZEI.baseUrl) as! String)!
        default:
            return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.ISCEI.baseUrl) as! String)!
        }
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .basicLogin:
            return "iscei/api/v1/auth/basic"
        case .getChallenge:
            return "iscei/api/v1/auth/generate-authentication-challenge"
        case .certificateLogin:
            return "iscei/api/v1/auth/mobile/certificate-login"
        case .verifyOTPCode:
            return "iscei/api/v1/auth/verify-otp"
        case .generateOTPCode:
            return "iscei/api/v1/auth/generate-otp"
        case .verifyLogin:
            return "mpozei/external/api/v1/mobile/verify-login"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .basicLogin,
                .getChallenge,
                .certificateLogin,
                .verifyLogin,
                .verifyOTPCode,
                .generateOTPCode:
            return .post
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .basicLogin(let input):
            params.append(input.toDict())
        case .getChallenge(let input):
            params.append(input.toDict())
        case .certificateLogin(let input):
            params.append(input.toDict())
        case .generateOTPCode(input: let input):
            params.append(input.toDict())
        case .verifyOTPCode(input: let input):
            params.append(input.toDict())
        case .verifyLogin(let input):
            params.append(input.toDict())
        }
        return params
    }
    
    // MARK: - Headers
    var headers: HTTPHeaders?  {
        var headers: HTTPHeaders = []
        headers.add(HTTPHeader.contentType("application/json"))
        headers.add(HTTPHeader(name: "Cookie", value: "KEYCLOAK_LOCALE=bg"))
        switch self {
        case .verifyLogin:
            if let bearerToken = StorageManager.keychain.getFor(key: .authToken) {
                headers.add(HTTPHeader.authorization(bearerToken: bearerToken))
            }
        default:
            break
        }
        return headers
    }
}
