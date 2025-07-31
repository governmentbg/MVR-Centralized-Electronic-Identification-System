//
//  URLRequestBuilder.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation
import Alamofire


protocol URLRequestBuilder: URLRequestConvertible, APIRequestHandler {
    // MARK: - Properties
    var mainURL: URL { get }
    var requestURL: URL { get }
    
    var path: String { get }
    
    var method: HTTPMethod { get }
    
    var parameters: Parameters? { get }
    
    var encoding: ParameterEncoding { get }
    
    var headers: HTTPHeaders? { get }
    
    var urlRequest: URLRequest { get }
    
    var isMultipartRequest: Bool { get }
}


// MARK: - Default configuration
extension URLRequestBuilder {
    var requestURL: URL {
        return mainURL.appendingPathComponent(path)
    }
    
    var defaultParams: Parameters {
        let params = Parameters()
        /**
         Default parameters can be added here
         */
        return params
    }
    
    var encoding: ParameterEncoding {
        switch method {
        case .get:
            return URLEncoding(boolEncoding: .literal)
        default:
            return JSONEncoding.default
        }
    }
    
    var urlRequest: URLRequest {
        var request = URLRequest(url: requestURL)
        request.httpMethod = method.rawValue
        if let headersList = headers {
            headersList.forEach { request.addValue($0.value, forHTTPHeaderField: $0.name) }
        }
        return request
    }
    
    func asURLRequest() throws -> URLRequest {
        if !isMultipartRequest {
            return try encoding.encode(urlRequest, with: parameters)
        }
        
        return try encoding.encode(urlRequest, with: nil)
    }
    
    internal var isMultipartRequest: Bool {
        return false
    }
}
