//
//  SystemRouter.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 31.01.25.
//

import Foundation
import Alamofire


enum SystemRouter: URLRequestBuilder {
    // MARK: - Endpoints
    case systemLocalisations(input: SystemLocalisationsRequest)
    
    // MARK: - BaseURL
    var mainURL: URL  {
        return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.PG.baseUrl) as! String)!
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .systemLocalisations(input: let request):
            return "assets/i18n/\(request.language.rawValue).json"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .systemLocalisations:
            return .get
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        return defaultParams
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
