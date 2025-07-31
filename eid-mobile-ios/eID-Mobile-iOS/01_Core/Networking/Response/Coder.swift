//
//  Coder.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation


struct Coder {
    /** JSON decoding to Array */
    static func decodeArray<T: Decodable>(of type: T.Type, from result: Data) -> [T]? {
        do {
            var decoded = [T]()
            decoded = try JSONDecoder().decode([T].self, from: result)
            return decoded
        } catch {
            return nil
        }
    }
    
    /** JSON decoding from Dict */
    static func decodeDict<T: Decodable>(of type: T.Type, from result: Data) -> T? {
        do {
            return try JSONDecoder().decode(T.self, from: result)
        } catch {
#if DEBUG
            debugPrint(error)
#endif
            return nil
        }
    }
    
    /** JSON decoding from Dict */
    static func decodeDict<T: Decodable>(of type: T.Type, from dict: [String: Any]) -> T? {
        do {
            let jsonData = try JSONSerialization.data(withJSONObject: dict)
            return try JSONDecoder().decode(T.self, from: jsonData)
        } catch {
#if DEBUG
            debugPrint(error)
#endif
            return nil
        }
    }
    
    /** JWT token decoding*/
    static func decodeJWTToken(_ jwt: String) throws -> [String: Any] {
        enum DecodeErrors: Error {
            case badToken
            case other
        }

        func base64Decode(_ base64: String) throws -> Data {
            let base64 = base64
                .replacingOccurrences(of: "-", with: "+")
                .replacingOccurrences(of: "_", with: "/")
            let padded = base64.padding(toLength: ((base64.count + 3) / 4) * 4, withPad: "=", startingAt: 0)
            guard let decoded = Data(base64Encoded: padded) else {
                throw DecodeErrors.badToken
            }
            return decoded
        }

        func decodeJWTPart(_ value: String) throws -> [String: Any] {
            let bodyData = try base64Decode(value)
            let json = try JSONSerialization.jsonObject(with: bodyData, options: [])
            guard let payload = json as? [String: Any] else {
                throw DecodeErrors.other
            }
            return payload
        }

        let segments = jwt.components(separatedBy: ".")
        return try decodeJWTPart(segments[1])
    }
    
    static func getJWTUser(fromToken jwt: String) -> JWTUser? {
        do {
            let jsonDict = try decodeJWTToken(jwt)
            let jsonData = try JSONSerialization.data(withJSONObject: jsonDict)
            return try JSONDecoder().decode(JWTUser.self, from: jsonData)
        } catch {
#if DEBUG
            debugPrint(error)
#endif
            return nil
        }
    }
}
