//
//  Encodable+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation


extension Encodable {
    func toDict() -> [String: Any]? {
        let jsonEncoder = JSONEncoder()
        let jsonData = try? jsonEncoder.encode(self)
        guard let data = jsonData else { return nil }
        return try? JSONSerialization.jsonObject(with: data, options: .mutableLeaves) as? [String: Any]
    }
    
    func queryParams() -> String {
        let queryParams = encodeDictionary(toDict() ?? [:])
        return "?\(queryParams)"
    }
    
    private func encodeDictionary(_ dictionary: [String: Any]) -> String {
        return dictionary
            .compactMap { (key, value) -> String? in
                if value is [String: Any] {
                    if let dictionary = value as? [String: Any] {
                        return encodeDictionary(dictionary)
                    }
                }
                else {
                    return "\(key)=\(value)"
                }
                
                return nil
            }
            .joined(separator: "&")
    }
}

extension KeyedEncodingContainer {
    mutating func encode(_ value: Double, forKey key: K) throws {
        try encode(String(describing: value.decimalValue), forKey: key)
    }
    
    mutating func encodeIfPresent(_ value: Double?, forKey key: K) throws {
        guard let value = value else { return }
        try encode(value.decimalValue, forKey: key)
    }
}
