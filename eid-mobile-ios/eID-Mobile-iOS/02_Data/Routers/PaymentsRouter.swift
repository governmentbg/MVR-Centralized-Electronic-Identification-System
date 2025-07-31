//
//  PaymentsRouter.swift
//  eID-Mobile-iOS
//
//  Created by Steliyan Hadzhidenev on 17.03.25.
//

import Foundation
import Alamofire

enum PaymentsRouter: URLRequestBuilder {
    // MARK: - Endpoints
    /// History
    case getHistory
    
    // MARK: - BaseURL
    var mainURL: URL  {
        return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.MPOZEI.baseUrl) as! String)!
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        /// History
        case .getHistory:
            return "mpozei/external/api/v1/payments"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .getHistory:
            return .get
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        let params = defaultParams
        switch self {
        case .getHistory:
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
}
