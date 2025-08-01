//
//  UserManager.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.02.24.
//

import Foundation
import UIKit


final class UserManager {
    // MARK: - User from token
    static func getUser() -> JWTUser? {
        guard let token = StorageManager.keychain.getFor(key: .authToken) else {
            return nil
        }
        return Coder.getJWTUser(fromToken: token)
    }
    
    static func removeUser() {
        StorageManager.keychain.clear(key: .authToken)
    }
    
    static var hasUser: Bool {
        guard getUser() != nil else {
            return false
        }
        return true
    }
    
    // MARK: - User from credentials
    static var credentials: UserCredentials? {
        return getUserCredentials()
    }
    
    static var biometricsAvailable: Bool {
        return credentials?.useBiometrics == true && BiometricProvider.biometricsAvailable
    }
    
    static var hasDevicePin: Bool {
        return credentials?.pin != nil
    }
    
    static func saveUserCredentials(credentials: UserCredentials) {
        var credentialsToSave: UserCredentials = credentials
        
        if let savedCredentials = getUserCredentials(), credentials.email == credentials.email {
            credentialsToSave = UserCredentials(email: credentials.email,
                                                password: credentials.password,
                                                pin: savedCredentials.pin,
                                                useBiometrics: savedCredentials.useBiometrics)
        }
        
        if let encodedCredentials = JSONUtilities.shared.encode(object: credentialsToSave) {
            StorageManager.keychain.save(key: .userCredentials, value: encodedCredentials)
        }
    }
    
    private static func getUserCredentials() -> UserCredentials? {
        guard let encodedCredentials = StorageManager.keychain.getFor(key: .userCredentials) else {
            return nil
        }
        
        if let credentials: UserCredentials = JSONUtilities.shared.decode(jsonString: encodedCredentials) {
            return credentials
        }
        
        return nil
    }
    
    static func removeUserCredentials() {
        StorageManager.keychain.clear(key: .userCredentials)
    }
    
    static func setDevicePin(pin: String?) {
        if var credentials = getUserCredentials() {
            credentials.pin = pin
            if let encodedCredentials = JSONUtilities.shared.encode(object: credentials) {
                StorageManager.keychain.save(key: .userCredentials, value: encodedCredentials)
            }
        }
    }
    
    static func setBiometrics(useBiometrics: Bool) {
        if var credentials = getUserCredentials() {
            credentials.useBiometrics = useBiometrics
            if let encodedCredentials = JSONUtilities.shared.encode(object: credentials) {
                StorageManager.keychain.save(key: .userCredentials, value: encodedCredentials)
            }
        }
    }
    
    // MARK: - Firebase ID
    static func getFirebaseId() -> String? {
        return StorageManager.keychain.getFor(key: .firebaseToken)
    }
    
    // MARK: - Mobile App Instance ID
    static func getMobileAppInstanceId() -> String? {
        return UIDevice().identifierForVendor?.uuidString
    }
}
