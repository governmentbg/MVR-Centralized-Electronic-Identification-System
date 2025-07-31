//
//  Set+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 9.02.24.
//

import Foundation


extension Set: @retroactive RawRepresentable where Element == String {
    public init?(rawValue: String) {
        guard
            let data = rawValue.data(using: .utf8),
            let result = try? JSONDecoder()
                .decode(Set<String>.self, from: data) else {
            return nil
        }
        self = result
    }
    
    public var rawValue: String {
        guard
            let data = try? JSONEncoder().encode(self),
            let result = String(data: data, encoding: .utf8)
        else { return "[]" }
        return result
    }
}
