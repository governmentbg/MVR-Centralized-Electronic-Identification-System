//
//  ServerErrorResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 19.07.24.
//

import Foundation


struct ServerErrorResponse: Codable {
    // MARK: - Properties
    var status: Int?
    var type: String?
    var title: String?
    var instance: String?
    var errors: [String]?
    
    public init(from decoder: Decoder) throws {
        let container = try decoder.container(keyedBy: CodingKeys.self)
        status =  try container.decodeIfPresent(Int.self, forKey: .status)
        type =  try container.decodeIfPresent(String.self, forKey: .type)
        title =  try container.decodeIfPresent(String.self, forKey: .title)
        instance =  try container.decodeIfPresent(String.self, forKey: .instance)
        
        if let stringError = try? container.decodeIfPresent(String.self, forKey: .errors) {
            errors = [stringError]
        } else if let arrayErrors = try? container.decodeIfPresent([String].self, forKey: .errors) {
            errors = arrayErrors
        } else if let errorDictionary = try? container.decodeIfPresent(Dictionary<String, [String]>.self, forKey: .errors) {
            errors = [errorDictionary.keys.first ?? ""]
        } else {
            errors = []
        }
    }
}

extension ServerErrorResponse: LocalizedError {
    var errorDescription: String? {
        var description = "error_message_try_again".localized()
        if let errorKey = errors?.first?.contains("_") == true ? errors?.first?.camelCased : errors?.first {
            GlobalLocalisations.errorLocalisations.forEach { item in
                if item.key == errorKey {
                    description = item.description
                }
            }
        }
        return description
    }
}
