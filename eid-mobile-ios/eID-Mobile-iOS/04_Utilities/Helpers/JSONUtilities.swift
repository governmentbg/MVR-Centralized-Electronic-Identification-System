//
//  JSONUtilities.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 10.06.25.
//

import Foundation



final class JSONUtilities {
    private init() { }
    static let shared = JSONUtilities()
    
    func encode<T: Codable>(object: T) -> String? {
        do {
            let encoder = JSONEncoder()
            let jsonData = try encoder.encode(object)
            return String(data: jsonData, encoding: .utf8)
        } catch let error {
            print(error.localizedDescription)
        }
        return nil
    }
    
    func decode<T: Decodable>(jsonString: String) -> T? {
        do {
            let decoder = JSONDecoder()
            if let data = jsonString.data(using: .utf8) {
                let decodedObject = try decoder.decode(T.self, from: data)
                return decodedObject
            }
        } catch {
            print("An error occurred while parsing JSON")
        }
        
        return nil
    }
}

