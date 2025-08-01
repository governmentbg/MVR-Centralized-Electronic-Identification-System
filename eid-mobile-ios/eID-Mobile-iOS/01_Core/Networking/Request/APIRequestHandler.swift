//
//  APIRequestHandler.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation
import Alamofire


/// Response completion handler beautified.
typealias CallResponse<T> = ((ServerResponse<T>) -> Void)?


/// API protocol, The alamofire wrapper
protocol APIRequestHandler: AlamofireResponseHandler {}


extension APIRequestHandler where Self: URLRequestBuilder {
    func send<T: Decodable>(_ decoder: T.Type?, data: [UploadData]? = nil, progress: ((Progress) -> Void)? = nil, completion: CallResponse<T>) {
        guard NetworkMonitor.shared.isConnected else {
            self.handleResponse(DataResponse(request: nil,
                                             response: nil,
                                             data: nil,
                                             metrics: nil,
                                             serializationDuration: .nan,
                                             result: .failure(AFError.sessionInvalidated(error: RequestError.noInternet))),
                                completion: completion)
            return
        }
        if let data = data,
           let decoder = decoder {
            uploadToServerWith(decoder,
                               data: data,
                               request: self,
                               parameters: self.parameters,
                               progress: progress,
                               completion: completion)
        } else {
            var request = NetworkClient.request(self)
            request = request.validate()
            
            if decoder == nil || decoder == Empty.self {
                // Handle responses different from 204 and 205 without body
                request.response { (response) in
                    self.handleResponse(response, completion: completion)
                }
            } else {
                request.responseData {(response) in
                    self.handleResponse(response, completion: completion)
                }
            }
        }
    }
}


extension APIRequestHandler {
    private func uploadToServerWith<T: Decodable>(_ decoder: T.Type, data: [UploadData], request: URLRequestConvertible, parameters: Parameters?, progress: ((Progress) -> Void)?, completion: CallResponse<T>) {
        NetworkClient.upload(multipartFormData: { (mul) in
            for singleObject in data {
                mul.append(singleObject.data,
                           withName: singleObject.name,
                           fileName: singleObject.fileName,
                           mimeType: singleObject.mimeType)
            }
            guard let parameters = parameters else { return }
            for (key, value) in parameters {
                mul.append("\(value)".data(using: String.Encoding.utf8)!, withName: key as String)
            }
        }, with: request)
        .responseData { (response) in
            self.handleResponse(response, completion: completion)
        }
    }
}
