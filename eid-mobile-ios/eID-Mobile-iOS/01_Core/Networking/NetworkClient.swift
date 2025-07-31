//
//  NetworkClient.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation
import Alamofire


final class NetworkClient {
    // MARK: - Properties
    private static let shared = NetworkClient()
    let session = Session()
    
    // MARK: - Static methods
    static func request(_ convertible: URLRequestConvertible) -> DataRequest {
        let request = shared.session.request(convertible)
        return shared.session.request(convertible)
    }
    
    static func upload(multipartFormData: @escaping (MultipartFormData) -> Void, with request: URLRequestConvertible) -> UploadRequest {
        return shared.session.upload(multipartFormData: multipartFormData, with: request)
    }
}
