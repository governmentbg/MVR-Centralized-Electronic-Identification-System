//
//  SigningRouter.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.12.23.
//

import Foundation
import Alamofire


enum SigningRouter: URLRequestBuilder {
    // MARK: - Endpoints
    case checkBoricaUser(input: CheckSingingUserRequest)
    case signWithBorica(input: BoricaSignRequest)
    case getBoricaStatus(input: BoricaTransactionRequest)
    case downloadBoricaSignature(input: BoricaTransactionRequest)
    case checkEurotrustUser(input: CheckSingingUserRequest)
    case signWithEurotrust(input: EurotrustSignRequest)
    case getEurotrustStatus(input: EurotrustTransactionRequest)
    case downloadEurotrustSignature(input: EurotrustTransactionRequest)
    
    // MARK: - BaseURL
    var mainURL: URL  {
        return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.PG.baseUrl) as! String)!
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .checkBoricaUser:
            return "signing/api/v1/Borica/user/check"
        case .signWithBorica:
            return "signing/api/v1/Borica/sign"
        case .getBoricaStatus:
            return "signing/api/v1/Borica/status"
        case .downloadBoricaSignature:
            return "signing/api/v1/Borica/download"
        case .checkEurotrustUser:
            return "signing/api/v1/Evrotrust/user/check"
        case .signWithEurotrust:
            return "signing/api/v1/Evrotrust/sign"
        case .getEurotrustStatus:
            return "signing/api/v1/Evrotrust/status"
        case .downloadEurotrustSignature:
            return "signing/api/v1/Evrotrust/download"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .getBoricaStatus,
                .downloadBoricaSignature,
                .getEurotrustStatus,
                .downloadEurotrustSignature:
            return .get
        case .checkEurotrustUser,
                .checkBoricaUser,
                .signWithBorica,
                .signWithEurotrust:
            return .post
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .signWithBorica(let input):
            params.append(input.toDict())
        case .getBoricaStatus(let input):
            params.append(input.toDict())
        case .downloadBoricaSignature(let input):
            params.append(input.toDict())
        case .signWithEurotrust(let input):
            params.append(input.toDict())
        case .getEurotrustStatus(let input):
            params.append(input.toDict())
        case .downloadEurotrustSignature(let input):
            params.append(input.toDict())
        case .checkEurotrustUser(let input):
            params.append(input.toDict())
        case .checkBoricaUser(let input):
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
