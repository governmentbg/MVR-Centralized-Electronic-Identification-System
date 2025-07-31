//
//  CertificateManager.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 20.02.24.
//

import Foundation


final class CertificateManager {
    // MARK: - Flags
    static var hasMobileEidStored: Bool {
        guard let _ = getCertificate(),
              let _ = StorageManager.keychain.getFor(key: .eidCertificatePIN)
        else {
            return false
        }
        return true
    }
    
    // MARK: - Certificate access
    @discardableResult static func storeCertificate(cert: String, certChain: [String], pin: String) -> Bool {
        guard !cert.isEmpty,
              !certChain.isEmpty
        else { return false }
        StorageManager.keychain.save(key: .eidCertificate, value: cert)
        StorageManager.keychain.save(key: .eidCertificateChain, value: certChain.joined(separator: ";"))
        StorageManager.keychain.save(key: .eidCertificatePIN, value: pin)
        guard let _ = StorageManager.keychain.getFor(key: .eidCertificate),
              let _ = StorageManager.keychain.getFor(key: .eidCertificateChain),
              let _ = StorageManager.keychain.getFor(key: .eidCertificatePIN)
        else {
            StorageManager.keychain.clear(key: .eidCertificate)
            StorageManager.keychain.clear(key: .eidCertificateChain)
            StorageManager.keychain.clear(key: .eidCertificatePIN)
            return false }
        return true
    }
    
    static func getCertificate() -> String? {
        return StorageManager.keychain.getFor(key: .eidCertificate)
    }
    
    static func getCertificateChain() -> [String]? {
        return StorageManager.keychain.getFor(key: .eidCertificateChain)?.components(separatedBy: ";")
    }
    
    // MARK: - Certificate PIN
    static func validateCertificatePin(_ pin: String) -> Bool {
        guard let storedPin = StorageManager.keychain.getFor(key: .eidCertificatePIN) else {
            return false
        }
        return pin == storedPin
    }
    
    static func validCertificateOwner() -> Bool {
        guard let cert = StorageManager.keychain.getFor(key: .eidCertificate),
              let base64EncodedData = cert.data(using: .utf8),
              let data = Data(base64Encoded: base64EncodedData),
              let decodedCert = String(data: data, encoding: .isoLatin1),
              let userID = UserManager.getUser()?.eidenityId else {
            return false
        }
        
        return decodedCert.contains(userID)
    }
    
    static func changeCertificatePin(oldPin: String, newPin: String) -> CertificatePinError? {
        guard !newPin.isEmpty,
              !oldPin.isEmpty,
              let storedPin = StorageManager.keychain.getFor(key: .eidCertificatePIN) else {
            return .missingPinForCertificate
        }
        
        guard oldPin == storedPin else {
            return .wrongCertificatePin
        }
        
        guard oldPin != newPin else {
            return .newPinSameAsOldPin
        }
        
        StorageManager.keychain.clear(key: .eidCertificatePIN)
        StorageManager.keychain.save(key: .eidCertificatePIN, value: newPin)
        return nil
    }
}
