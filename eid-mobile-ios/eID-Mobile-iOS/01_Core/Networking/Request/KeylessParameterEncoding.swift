//
//  KeylessParameterEncoding.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 17.10.23.
//

import Foundation
import Alamofire


/// Uses `JSONSerialization` to create a JSON representation of the parameters object, which is set as the body of the
/// request. The `Content-Type` HTTP header field of an encoded request is set to `application/json`.
public struct KeylessParameterEncoding: ParameterEncoding {
    public enum Error: Swift.Error {
        case invalidJSONObject
    }
    
    // MARK: Properties
    /// Returns a `KeylessParameterEncoding` instance with default writing options.
    public static var `default`: KeylessParameterEncoding { KeylessParameterEncoding() }
    
    /// Returns a `KeylessParameterEncoding` instance with `.prettyPrinted` writing options.
    public static var prettyPrinted: KeylessParameterEncoding { KeylessParameterEncoding(options: .prettyPrinted) }
    
    /// The options for writing the parameters as JSON data.
    public let options: JSONSerialization.WritingOptions
    
    // MARK: Initialization
    /// Creates an instance using the specified `WritingOptions`.
    ///
    /// - Parameter options: `JSONSerialization.WritingOptions` to use.
    public init(options: JSONSerialization.WritingOptions = []) {
        self.options = options
    }
    
    // MARK: Encoding
    public func encode(_ urlRequest: URLRequestConvertible, with parameters: Parameters?) throws -> URLRequest {
        var urlRequest = try urlRequest.asURLRequest()
        
        // Not applicable when there are more than 1 parameters
        guard let parameters = parameters,
              parameters.count == 1,
              let parameter = parameters.first
        else {
            return urlRequest
        }
        
        // Ignores the key, only uses value
        guard JSONSerialization.isValidJSONObject(parameter.value) else {
            throw AFError.parameterEncodingFailed(reason: .jsonEncodingFailed(error: Error.invalidJSONObject))
        }
        
        do {
            let data = try JSONSerialization.data(withJSONObject: parameter.value, options: options)
            if urlRequest.headers["Content-Type"] == nil {
                urlRequest.headers.update(.contentType("application/json"))
            }
            urlRequest.httpBody = data
        } catch {
            throw AFError.parameterEncodingFailed(reason: .jsonEncodingFailed(error: error))
        }
        
        return urlRequest
    }
    
    /// Encodes any JSON compatible object into a `URLRequest`.
    ///
    /// - Parameters:
    ///   - urlRequest: `URLRequestConvertible` value into which the object will be encoded.
    ///   - jsonObject: `Any` value (must be JSON compatible` to be encoded into the `URLRequest`. `nil` by default.
    ///
    /// - Returns:      The encoded `URLRequest`.
    /// - Throws:       Any `Error` produced during encoding.
    public func encode(_ urlRequest: URLRequestConvertible, withJSONObject jsonObject: Any? = nil) throws -> URLRequest {
        var urlRequest = try urlRequest.asURLRequest()
        
        guard let jsonObject = jsonObject else {
            return urlRequest
        }
        
        guard JSONSerialization.isValidJSONObject(jsonObject) else {
            throw AFError.parameterEncodingFailed(reason: .jsonEncodingFailed(error: Error.invalidJSONObject))
        }
        
        do {
            let data = try JSONSerialization.data(withJSONObject: jsonObject, options: options)
            if urlRequest.headers["Content-Type"] == nil {
                urlRequest.headers.update(.contentType("application/json"))
            }
            urlRequest.httpBody = data
        } catch {
            throw AFError.parameterEncodingFailed(reason: .jsonEncodingFailed(error: error))
        }
        
        return urlRequest
    }
}
