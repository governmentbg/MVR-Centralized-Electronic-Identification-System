//
//  ServerSettings.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 27.03.25.
//

import Foundation


enum GlobalLocalisations {
    static var logLocalisations = [PGLocalisedItem]()
    static var approvalRequestTypes = [PGLocalisedItem]()
    static var errorLocalisations = [PGLocalisedItem]()
    static var didFinishLoading: (() -> ())?
    
    static func getServerSettings() {
        let language = LanguageManager.preferredLanguage ?? .bg
        SystemRouter.systemLocalisations(input: SystemLocalisationsRequest(language: language))
            .send(PGSystemLocalisations.self) { response in
                switch response {
                case .success(let response):
                    if let localisations = response {
                        for (key, value) in localisations.logs {
                            GlobalLocalisations.logLocalisations.append(PGLocalisedItem(key: key, description: value))
                        }
                        for (key, value) in localisations.approvalRequestTypes {
                            GlobalLocalisations.approvalRequestTypes.append(PGLocalisedItem(key: key, description: value))
                        }
                        for (key, value) in localisations.errors {
                            GlobalLocalisations.errorLocalisations.append(PGLocalisedItem(key: key, description: value))
                        }
                    }
                case .failure(_): break
                }
                didFinishLoading?()
            }
    }
}
