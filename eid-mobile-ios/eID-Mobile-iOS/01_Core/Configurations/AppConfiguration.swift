//
//  AppConfiguration.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation

/**
 Configuration stuct is  used to retrive configuration parameters from the config file.
 The config file is config.plist .
 
 Multiple environments are supported.
 Environments are places under the "modes" key.
 Default environment is "Debug".
 */
struct AppConfiguration {
    /**
     Retrieves the value stored under the key from the config file.
     First it checkes the current environment and if no value is present it checks the root of the config file.
     @param key the key for the requested value
     @return the value if found or nil
     */
    static func get(key: String) -> Any? {
        let envVariableValue = ProcessInfo.processInfo.environment[key]
        if envVariableValue != nil {
            return envVariableValue
        } else if  let pathToFile = AppConfiguration.configFile(),
                   let dict = NSDictionary(contentsOfFile: pathToFile),
                   let newDict = dict as? [String: Any] {
            return newDict[key]
        }
        return nil
    }
    
    static func get(keychainKey: KeychainKey) -> Any? {
        let envVariableValue = ProcessInfo.processInfo.environment[keychainKey.rawValue]
        if envVariableValue != nil {
            return envVariableValue
        } else if  let pathToFile = AppConfiguration.configFile(),
                   let dict = NSDictionary(contentsOfFile: pathToFile),
                   let newDict = dict as? [String: Any] {
            return newDict[keychainKey.rawValue]
        }
        return nil
    }
    
    /**
     Sets the environment. The environment is saved in the user defaults.
     @param env the new environment.
     */
    static func initEnvironment() {
        guard currentEnvironment() == .unknown else {
            return
        }
        setEnvironment(env: .digitallDev)
    }
    
    /**
     Sets the environment. The environment is saved in the user defaults.
     @param env the new environment.
     */
    static func setEnvironment(env: AppEnvironment) {
        UserDefaults.standard.set(env.rawValue, 
                                  forKey: Constants.ConfigurationStrings.environment)
    }
    
    /**
     Gets the enviroment. If the enviroment is saved in user defaults, it will return it. If environment is not found in UserDefault environment mode will be taken from Build environments
     */
    static func currentSavedEnvironment() -> AppEnvironment? {
        guard let storedEnv = UserDefaults.standard.string(forKey: Constants.ConfigurationStrings.environment)
        else {
            return nil
        }
        return AppEnvironment(rawValue: storedEnv)
    }
    
    static func currentEnvironment() -> AppEnvironment {
        if let env = currentSavedEnvironment(),
           env != .unknown {
            return env
        } else {
            var env = AppEnvironment.unknown
#if DIGITALL_DEV
            env = AppEnvironment.digitallDev
#elseif PRODUCTION
            env = AppEnvironment.production
#elseif MVR_DEV
            env = AppEnvironment.mvrDev
#elseif MVR_TEST
            env = AppEnvironment.mvrTest
#elseif MVR_STAGE
            env = AppEnvironment.mvrStage
#endif
            if env != AppEnvironment.unknown {
                setEnvironment(env: env)
            }
            return env
        }
    }
}


private extension AppConfiguration {
    static func configFile() -> String? {
        let currentEnvironment = currentEnvironment()
        return Bundle.main.path(forResource: "config_\(currentEnvironment.rawValue)",
                                ofType: "plist")
    }
}
