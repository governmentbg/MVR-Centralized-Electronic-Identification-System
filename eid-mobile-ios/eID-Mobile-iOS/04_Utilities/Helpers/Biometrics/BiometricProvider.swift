//
//  BiometricProvider.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.06.25.
//

import LocalAuthentication


class BiometricProvider {
    static let context = LAContext()
    
    static var biometricType: BiometricType {
        let authContext = LAContext()
        let _ = authContext.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: nil)
        switch(authContext.biometryType) {
        case .touchID:
            return .touch
        case .faceID:
            return .face
        default:
            return .none
        }
    }
    
    static var biometricsAvailable: Bool {
        var error: NSError?
        return context.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: &error)
    }
    
    static func authenticate(completion: @escaping (Bool, BiometricError?) -> ()) {
        var error: NSError?
        
        if context.canEvaluatePolicy(.deviceOwnerAuthenticationWithBiometrics, error: &error) {
            context.evaluatePolicy(.deviceOwnerAuthenticationWithBiometrics,
                                   localizedReason: "biometrics_use_reson_description".localized())
            { success, authenticationError in
                if success {
                    completion(success, nil)
                } else {
                    if let errorCode = (authenticationError as? NSError)?.code {
                        let biometricsError = BiometricError.getBy(id: errorCode)
                        switch biometricsError {
                        case .message(_):
                            completion(false, BiometricError.message(authenticationError?.localizedDescription ?? ""))
                        default:
                            completion(false, biometricsError)
                        }
                    } else {
                        completion(false, BiometricError.message(authenticationError?.localizedDescription ?? ""))
                    }
                }
            }
        } else {
            completion(false, BiometricError.biometryNotAvailable)
        }
    }
}

enum BiometricType {
    case none
    case touch
    case face
    
    var description: String {
        switch self {
        case .face:
            return "device_security_option_face_id".localized()
        case .touch:
            return "device_security_option_touch_id".localized()
        default:
            return ""
        }
    }
}
