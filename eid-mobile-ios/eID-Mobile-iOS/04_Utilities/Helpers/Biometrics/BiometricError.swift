//
//  BiometricError.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.06.25.
//

import Foundation



enum BiometricError: Error, Equatable {
    var description: String {
        switch self {
        case .biometryNotAvailable:
            return "biometry_not_available".localized()
        case .biometryNotEnrolled:
            return "biometry_not_enrolled".localized()
        case .biometryLockout:
            return "biometry_lockout".localized()
        case .failed:
            return "biometrics_failed".localized()
        case .cancelled:
            return "biometrics_cancelled".localized()
        case .fallback:
            return ""
        case .message(let message):
            return message
        }
    }
    
    static func getBy(id: Int) -> BiometricError {
        switch id {
        case -1:
            return .failed
        case -2:
            return .cancelled
        case -3:
            return .fallback
        case -6:
            return .biometryNotEnrolled
        case -7:
            return .biometryLockout
        case -11:
            return .biometryNotAvailable
        default:
            return .message("")
        }
    }
    
    case biometryNotAvailable
    case biometryNotEnrolled
    case biometryLockout
    case failed
    case cancelled
    case fallback
    case message(String)
}
