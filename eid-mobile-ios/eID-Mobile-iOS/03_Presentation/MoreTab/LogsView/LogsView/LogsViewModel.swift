//
//  LogsViewModel.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 18.10.24.
//

import Foundation


class LogsViewModel: ObservableObject {
    /// MARK: Properties
    @Published var localisedDescriptions = [LocalisedLog]()
    
    /// MARK: Helpers
    func getLogLocalisations() {
        if localisedDescriptions.isEmpty {
            let currentLanguage = LanguageManager.preferredLanguage ?? .bg
            for description in GlobalLocalisations.logLocalisations {
                let localisedDescription = LocalisedLog(key: description.key, descriptions: [
                    currentLanguage.rawValue: description.description,
                ])
                localisedDescriptions.append(localisedDescription)
            }
        }
        
        //        getLocalisedLogDescriptionsFor(.en) { [weak self] enDescriptions in
        //            self?.getLocalisedLogDescriptionsFor(.bg) { bgDescriptions in
        //                for description in enDescriptions ?? [] {
        //                    if let bgDescription = bgDescriptions?.first(where: { $0.key == description.key }) {
        //                        let localisedDescription = LocalisedLog(key: description.key, descriptions: [
        //                            Language.en.rawValue: description.description,
        //                            Language.bg.rawValue: bgDescription.description
        //                        ])
        //                        self?.localisedDescriptions.append(localisedDescription)
        //                    }
        //                }
        //            }
        //        }
    }
}
