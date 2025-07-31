//
//  LogsRouter.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation
import Alamofire


enum LogsRouter: URLRequestBuilder {
    // MARK: - Endpoints
    case logsFromMe(input: LogsRequest)
    case logsToMe(input: LogsRequest)
    
    // MARK: - BaseURL
    var mainURL: URL  {
        return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.PG.baseUrl) as! String)!
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .logsFromMe:
            return "pjs/api/v1/Log/from"
        case .logsToMe:
            return "pjs/api/v1/Log/to"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .logsFromMe,
                .logsToMe:
            return .post
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .logsFromMe(let input):
            params.append(input.toDict())
        case .logsToMe(let input):
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
