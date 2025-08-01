//
//  ApprovalRequestRouter.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 30.07.24.
//

import Foundation

import Foundation
import Alamofire


enum ApprovalRequestRouter: URLRequestBuilder {
    // MARK: - Endpoints
    case getApprovalRequests
    case setApprovalRequestState(input: ApprovalRequestStateRequest)
    
    // MARK: - BaseURL
    var mainURL: URL {
        return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.ISCEI.baseUrl) as! String)!
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .getApprovalRequests:
            return "iscei/api/v1/approval-request/user"
        case .setApprovalRequestState(input: let input):
            return "iscei/api/v1/approval-request/outcome\(input.queryParams.queryParams())"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .getApprovalRequests:
            return .get
        case .setApprovalRequestState:
            return .post
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .getApprovalRequests:
            break
        case .setApprovalRequestState(input: let input):
            params.append(input.bodyParams.toDict())
        }
        return params
    }
    
    // MARK: - Headers
    var headers: HTTPHeaders? {
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
        case .setApprovalRequestState:
            if let fullPath = URL(string: mainURL.appendingPathComponent(path).absoluteString.removingPercentEncoding ?? "") {
                return fullPath
            }
        default: break
        }
        return  mainURL.appendingPathComponent(path)
    }
}
