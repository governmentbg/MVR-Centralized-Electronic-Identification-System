//
//  ResponseHandler.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation
import Alamofire


protocol AlamofireResponseHandler {
    /// Handles request response, never called anywhere but APIRequestHandler
    ///
    /// - Parameters:
    ///   - response: response from network request, for now alamofire Data response
    ///   - completion: completing processing the json response, and delivering it in the completion handler
    func handleResponse<T: Decodable>(_ response: DataResponse<Data?, AFError>, completion: CallResponse<T>)
    func handleResponse<T: Decodable>(_ response: DataResponse<Data, AFError>, completion: CallResponse<T>)
}


extension AlamofireResponseHandler {
    // MARK: - Public Methods
    func handleResponse<T: Decodable>(_ response: DataResponse<Data?, AFError>, completion: CallResponse<T>) {
        switch response.result {
        case .failure(let afError):
            handleFailure(statusCode: response.response?.statusCode ?? 0,
                          data: response.data,
                          error: afError,
                          completion: completion)
        case .success(let value):
            handleSuccess(data: value,
                          completion: completion)
        }
    }
    
    func handleResponse<T: Decodable>(_ response: DataResponse<Data, AFError>, completion: CallResponse<T>) {
        switch response.result {
        case .failure(let afError):
            handleFailure(statusCode: response.response?.statusCode ?? 0,
                          data: response.data,
                          error: afError,
                          completion: completion)
        case .success(let value):
            handleSuccess(data: value,
                          completion: completion)
        }
    }
    
    // MARK: - Private Methods
    private func handleFailure<T: Decodable>(statusCode: Int, data: Data?, error: AFError?, completion: CallResponse<T>) {
        guard (error?.underlyingError as? RequestError) == nil else {
            if let requestError = error?.underlyingError as? RequestError,
               requestError == .noInternet {
                completion?(ServerResponse<T>.failure(RequestError.noInternet))
            } else {
                completion?(ServerResponse<T>.failure(ServerError.generalError))
            }
            return
        }
        
        switch statusCode {
        case ErrorCode.unauthorized.rawValue:
            NotificationCenter.default.post(name: .unauthorizedRequest, object: nil)
            return
        case ErrorCode.serverFailureLowerBound.rawValue...ErrorCode.serverFailureUpperBound.rawValue,
            ErrorCode.notFound.rawValue:
            NotificationCenter.default.post(name: .serverFailure, object: nil)
            completion?(ServerResponse<T>.failure(ServerError.generalError))
            return
        default:
            break
        }
        do {
            let serverError = try ServerErrorResponse(data: data ?? Data())
            completion?(ServerResponse<T>.failure(serverError))
        } catch {
            completion?(ServerResponse<T>.failure(ServerError.generalError))
        }
    }
    
    private func handleSuccess<T: Decodable>(data: Data?, completion: CallResponse<T>) {
        guard let newData = data else {
            completion?(ServerResponse<T>.success(nil))
            return
        }
        if T.self == Bool.self {
            completion?(ServerResponse<T>.success(true as? T))
            return
        }
        if T.self == Void.self {
            completion?(ServerResponse<T>.success(() as? T))
            return
        }
        if T.self == Data.self {
            completion?(ServerResponse<T>.success(newData as? T))
            return
        }
        do {
            let modules = try T(data: newData)
            completion?(ServerResponse<T>.success(modules))
        } catch(let error) {
#if DEBUG
            print(error)
            print("===== WARNING: Empty result or unable to parse as \(String(describing: T.self)) : /n \(newData)")
#endif
            completion?(ServerResponse<T>.success(nil))
        }
    }
}


