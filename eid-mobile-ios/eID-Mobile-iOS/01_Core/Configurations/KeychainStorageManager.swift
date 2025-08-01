//
//  KeychainStorageManager.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 9.10.23.
//

import Foundation
import KeychainAccess
import Security


// MARK: - Keys
enum KeychainKey: String {
    // App
    case appLanguage = "appLanguage"
    
    // User
    case id = "id"
    case email = "email"
    
    // URLs
    case baseUrlPG = "BASE_URL_PG"
    case baseUrlMPOZEI = "BASE_URL_MPOZEI"
    case baseUrlISCEI = "BASE_URL_ISCEI"
    case baseUrlRAEICEI = "BASE_URL_RAEICEI"
    case baseUrlPAYMENT = "BASE_URL_PAYMENT"
    case paymentCode = "PAYMENT_CODE"
    
    // EID Certificate
    case eidCertificatePIN = "eidCertificatePIN"
    case eidCertificate = "eidCertificate"
    case eidCertificateChain = "eidCertificateChain"
    case csrPrivateKey = "csrPrivateKey"
    
    // Tokens
    case authToken = "authToken"
    case firebaseToken = "firebaseToken"
    case userCredentials = "eIDUserCredentials"
    
    // Launch options
    case remoteNotification = "remoteNotification"
}


// MARK: - Storage protocol
protocol Storage {
    func getFor(key: KeychainKey) -> String?
    func save(with key: Key)
    func clearStorage()
}


// MARK: - Key protocol
protocol Key {
    var type: String {get set}
    var value: String? {get set}
}


// MARK: - Keychain Key
struct KeychainKeyModel: Key {
    var type: String
    var value: String?
    
    init(value: String?, type: String) {
        self.type = type
        self.value = value
    }
    
    init(key: KeychainKey, value: String?) {
        self.type = key.rawValue
        self.value = value
    }
}


// MARK: - Storage Manager
struct StorageManager {
    // MARK: - Properties
    static let keychain = StorageManager(storage: KeychainStorageManager())
    let storage: Storage
    
    // MARK: - Init
    /** Init with custom storage */
    init(storage: Storage) {
        self.storage = storage
    }
    
    // MARK: - Save/Get
    /** Save key in Storage */
    func save(with key: Key) {
        storage.save(with: key)
    }
    
    /** Get keys from Storage */
    func getFor(key: KeychainKey) -> String? {
        return storage.getFor(key: key)
    }
    
    /** Save value for defined KeychainKey in Storage */
    func save(key: KeychainKey, value: String?) {
        let key = KeychainKeyModel(key: key, value: value)
        save(with: key)
    }
    
    /** Clear value for defined KeychainKey in Storage */
    func clear(key: KeychainKey) {
        save(key: key, value: nil)
    }
}


// MARK: - Storage instance of the app
/** KeyChain Storage manager for tokens */
fileprivate struct KeychainStorageManager: Storage {
    // MARK: - Properties
    let keychain = Keychain(service: Bundle.main.bundleIdentifier ?? "com.Digitall.eID").accessibility(.whenUnlocked)
    /** Sensitive information to read from .config file and store in Keychain on initial app launch.
     Sensitive information is recommended to be extracted from config.plist, stored in keyvault and dinamically populated when preparing a build for release.  */
    let configKeys: [KeychainKey] = []
    
    // MARK: - Init
    init() {
        checkIfFirstRunDeleteKeychainEntries()
        initConfigKeysIfNeeded()
    }
    
    // MARK: - Save/Get
    func save(with key: Key) {
        keychain[key.type] = key.value
    }
    
    func getFor(key: KeychainKey) -> String? {
        return keychain[key.rawValue]
    }
    
    func clearStorage() {
        do {
            try keychain.removeAll()
            initConfigKeysIfNeeded()
        } catch _ {
        }
    }
    
    // MARK: - Private methods
    private func checkIfFirstRunDeleteKeychainEntries() {
        let defaults = UserDefaults.standard
        if !defaults.bool(forKey: Constants.KeychainKeys.hasRunBefore) {
            clearStorage()
            defaults.set(true, forKey: Constants.KeychainKeys.hasRunBefore)
        }
    }
    
    private func initConfigKeysIfNeeded() {
        for key in configKeys {
            if let value = AppConfiguration.get(key: key.rawValue) as? String,
               getFor(key: key) != value {
                save(with: KeychainKeyModel(key: key, value: value))
            }
        }
    }
}
