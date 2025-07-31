//
//  Decodable+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation


extension Decodable {
    /** Note: Decoder uses .convertFromSnakeCase as a keyDecodingStrategy. There is no need to define Coding keys in the Codable object with snake-case values. */
    init(data: Data) throws {
        let decoder = JSONDecoder()
        decoder.keyDecodingStrategy = .convertFromSnakeCase
        self = try decoder.decode(Self.self, from: data)
    }
    
    init(json: String) throws {
        let data = Data(json.utf8)
        let decoder = JSONDecoder()
        decoder.keyDecodingStrategy = .convertFromSnakeCase
        self = try decoder.decode(Self.self, from: data)
    }
}
