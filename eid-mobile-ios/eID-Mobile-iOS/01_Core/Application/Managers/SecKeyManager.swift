//
//  SecKeyManager.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.02.24.
//

import X509
import CryptoKit
import Security
import _CryptoExtras
import Foundation
import OpenSSL

final class SecKeyManager {
    // MARK: - Private key - P256
    private static func getPrivateKey(forceUpdate: Bool = false) -> P256.Signing.PrivateKey {
        if !forceUpdate,
           let storedCert = StorageManager.keychain.getFor(key: .eidCertificate),
           !storedCert.isEmpty,
           let storedPrivateKeyPEM = StorageManager.keychain.getFor(key: .csrPrivateKey) {
            do {
                let storedPrivateKey = try P256.Signing.PrivateKey(pemRepresentation: storedPrivateKeyPEM)
                return storedPrivateKey
            } catch {
                let newPrivateKey = P256.Signing.PrivateKey()
                StorageManager.keychain.save(key: .csrPrivateKey, value: newPrivateKey.pemRepresentation)
                return newPrivateKey
            }
        } else {
            let newPrivateKey = P256.Signing.PrivateKey()
            StorageManager.keychain.save(key: .csrPrivateKey, value: newPrivateKey.pemRepresentation)
            return newPrivateKey
        }
    }
    
    
    // MARK: - Private key - RSA
    private static func getRSAPrivateKey(forceUpdate: Bool = false) -> _RSA.Signing.PrivateKey? {
        if !forceUpdate,
           let storedCert = StorageManager.keychain.getFor(key: .eidCertificate),
           !storedCert.isEmpty,
           let storedPrivateKeyPEM = StorageManager.keychain.getFor(key: .csrPrivateKey) {
            do {
                let storedPrivateKey = try _RSA.Signing.PrivateKey(pemRepresentation: storedPrivateKeyPEM)
                return storedPrivateKey
            } catch {
                return generateNewRSAPrivateKey()
            }
        } else {
            return generateNewRSAPrivateKey()
        }
    }
    
    private static func generateNewRSAPrivateKey() -> _RSA.Signing.PrivateKey? {
        do {
            let newPrivateKey = try _RSA.Signing.PrivateKey(keySize: .bits3072)
            StorageManager.keychain.save(key: .csrPrivateKey, value: newPrivateKey.pemRepresentation)
            return newPrivateKey
        } catch let error {
            print("RSA private key generation error: \(error.localizedDescription)")
            return nil
        }
    }
    
    
    // MARK: - Signature
    static func getSignature(for challenge: String) -> String? {
        guard let privateKey = getRSAPrivateKey() else { return nil }
        let challengeBytes = Array(challenge.utf8)
        do {
            let signature = try privateKey.signature(for: challengeBytes)
            return signature.rawRepresentation.base64EncodedString()
        } catch {
            return nil
        }
    }
    
    
    // MARK: - CSR
    static func generateCSR(useNewKey: Bool = false) throws -> String? {
        guard let user = UserManager.getUser() else { return "" }
        
        guard let privateRSAKey = getRSAPrivateKey(forceUpdate: useNewKey) else {
            return nil
        }
        
        return buildCSR(forPrivateKey: privateRSAKey.derRepresentation, user: user)
        
        //        let subject = try DistinguishedName([.init(type: .NameAttributes.countryName,
        //                                                   printableString: "BG"),
        //                                             .init(type: .NameAttributes.commonName, // Кирилица
        //                                                   utf8String: user.nameCyrillic),
        //                                             .init(type: .NameAttributes.givenName, // Име - латиница
        //                                                   utf8String: user.givenNameLatin ?? ""),
        //                                             .init(type: .NameAttributes.surname, // Фамилия - латиница
        //                                                   utf8String: user.familyNameLatin ?? ""),
        //                                             .init(type: .NameAttributes.serialNumber,
        //                                                   utf8String: "PI:BG-\(user.eidenityId ?? "")") // eidentity ID - userId
        //        ])
        //
        //        let privateKeyCertificate = Certificate.PrivateKey(privateRSAKey)
        //        do {
        //            let csr = try CertificateSigningRequest(version: .v1,
        //                                                    subject: subject,
        //                                                    privateKey: privateKeyCertificate,
        //                                                    attributes: CertificateSigningRequest.Attributes(),
        //                                                    signatureAlgorithm: .sha256WithRSAEncryption)
        //            guard csr.publicKey.isValidSignature(csr.signature, for: csr) else { return  ""}
        //            /// Get CSR text
        //            let csrText = try csr.serializeAsPEM(discriminator: CertificateSigningRequest.defaultPEMDiscriminator).pemString
        //            return csrText
        //        } catch let error {
        //            print("CSR generation error: \(error.localizedDescription)")
        //        }
        //        return nil
    }
    
    private static func buildCSR(forPrivateKey: Data, user: JWTUser) -> String? {
        OpenSSL_add_all_algorithms()
        
        var request: OpaquePointer?
        var key: OpaquePointer?
        var name: OpaquePointer?
        var rsa: OpaquePointer?
        var csrAsPem: String?
        
        csrAsPem = forPrivateKey.withUnsafeBytes { unsafeBytes in
            var bits = unsafeBytes.bindMemory(to: UInt8.self).baseAddress
            let length = forPrivateKey.count
            
            request = X509_REQ_new()
            key = EVP_PKEY_new()
            
            defer { EVP_PKEY_free(key) }
            
            d2i_RSAPrivateKey(&rsa, &bits, length)
            EVP_PKEY_assign(key, EVP_PKEY_RSA, UnsafeMutablePointer(rsa))
            X509_REQ_set_version(request, 0)
            
            name = X509_REQ_get_subject_name(request)
            X509_NAME_add_entry_by_NID(name, NID_commonName, MBSTRING_UTF8, user.nameCyrillic, -1, -1, 0)
            X509_NAME_add_entry_by_NID(name, NID_givenName, MBSTRING_UTF8,  user.givenNameLatin ?? "", -1, -1, 0)
            X509_NAME_add_entry_by_NID(name, NID_surname, MBSTRING_UTF8, user.familyNameLatin ?? "", -1, -1, 0)
            X509_NAME_add_entry_by_NID(name, NID_countryName, MBSTRING_UTF8, "BG", -1, -1, 0)
            X509_NAME_add_entry_by_NID(name, NID_serialNumber, MBSTRING_UTF8, "PI:BG-\(user.eidenityId ?? "")", -1, -1, 0)
            
            X509_REQ_set_pubkey(request, key)
            
            // Sign algorithm setup
            let signCtx = EVP_MD_CTX_new()
            var signPkCtx: OpaquePointer?
            
            defer { EVP_MD_CTX_free(signCtx) }
            
            EVP_DigestSignInit(signCtx, &signPkCtx, EVP_sha256(), nil, key)
            EVP_PKEY_CTX_ctrl_str(signPkCtx, "rsa_padding_mode", "pss")
            EVP_PKEY_CTX_ctrl_str(signPkCtx, "rsa_pss_saltlen", "32")
            X509_REQ_sign_ctx(request, signCtx)
            
            guard X509_REQ_verify(request, key) == 1 else {
                return nil
            }
            
            guard let out = BIO_new(BIO_s_mem()) else {
                return nil
            }
            
            defer { BIO_free(out) }
            
            PEM_write_bio_X509_REQ(out, request)
            
            return OpenSSLUtils.bioToString(bio: out)
        }
        
        return csrAsPem
    }
}
