//
//  LanguageManager.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 27.10.23.
//

import Foundation


final class LanguageManager {
    static func initLanguage() {
        let defaults = UserDefaults.standard
        if defaults.value(forKey: "alreadyLaunch") as? Bool == nil
            || defaults.value(forKey: "alreadyLaunch") as? Bool == false {
            defaults.set(["bg-BG", "en-BG"], forKey: "AppleLanguages")
            defaults.set(true, forKey: "alreadyLaunch")
        }
    }
    
    static var preferredLanguage: Language? {
        guard let preferredLanguage = Bundle.main.preferredLocalizations.first,
              let language = Language(rawValue: preferredLanguage) else {
            return nil
        }
        
        return language
    }
}
