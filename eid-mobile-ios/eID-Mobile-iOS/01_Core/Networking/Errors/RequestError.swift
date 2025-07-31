//
//  RequestError.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 8.02.24.
//

import Foundation


public enum RequestError: Error {
    case noInternet
}

extension RequestError: LocalizedError {
    public var errorDescription: String? {
        switch self {
        case .noInternet:
            return "error_no_internet".localized()
        }
    }
}
