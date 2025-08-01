//
//  String+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 27.10.23.
//

import Foundation

// MARK: - Localization
extension String {
    func localized() -> String {
        return NSLocalizedString(self, comment: "\(self)_comment")
    }
    
    func localized(_ args: [CVarArg]) -> String {
        return String(format: localized(), args)
    }
    
    func localized(_ args: CVarArg...) -> String {
        return String(format: localized(), args)
    }
}

// MARK: - Substring
extension String {
    func substring(from: Int, take: Int) -> String {
        let start = index(startIndex, offsetBy: from)
        let end = index(startIndex, offsetBy: from + take)
        let range = start..<end
        
        return String(self[range])
    }
    
    func substring(from: Int, to: Int) -> String {
        let start = index(startIndex, offsetBy: from)
        let end = index(startIndex, offsetBy: to)
        let range = start..<end
        
        return String(self[range])
    }
}


// MARK: - Date
extension String {
    func toDate(withStringFormat format: String) -> Date? {
        guard let utcTimeZone = TimeZone.init(secondsFromGMT: 0) else {
            return nil
        }
        
        let dateFormatter = DateFormatter()
        dateFormatter.timeZone = utcTimeZone
        dateFormatter.dateFormat = format
        return dateFormatter.date(from: self)
    }
    
    func toDate(withFormats formats: [DateFormat] = [.iso8601UTC, .iso8601NoFractionsUTC, .iso8601Date]) -> Date? {
        let parsedDate: Date? = nil
        for format in formats {
            if let newDate = toDate(withStringFormat: format.rawValue) {
                return newDate
            }
        }
        return parsedDate
    }
}

// MARK: - Base64
extension String {
    func fromBase64() -> String? {
        guard let data = Data(base64Encoded: self) else {
            return nil
        }
        return String(data: data, encoding: .utf8)
    }
    
    func toBase64() -> String {
        return Data(self.utf8).base64EncodedString()
    }
    
}


// MARK: - HEX
extension String {
    /// Create `Data` from hexadecimal string representation
    ///
    /// This creates a `Data` object from hex string. Note, if the string has any spaces or non-hex characters (e.g. starts with '<' and with a '>'), those are ignored and only hex characters are processed.
    ///
    /// - returns: Data represented by this hexadecimal string.
    var hexadecimal: Data? {
        var data = Data(capacity: count / 2)
        let regex = try! NSRegularExpression(pattern: "[0-9a-f]{1,2}", options: .caseInsensitive)
        regex.enumerateMatches(in: self, range: NSRange(startIndex..., in: self)) { match, _, _ in
            let byteString = (self as NSString).substring(with: match!.range)
            let num = UInt8(byteString, radix: 16)!
            data.append(num)
        }
        guard data.count > 0 else { return nil }
        return data
    }
}


// MARK: - Digits from String
extension String {
    var digits: [Int] {
        var result = [Int]()
        for char in self {
            if let number = Int(String(char)) {
                result.append(number)
            }
        }
        return result
    }
}

// MARK: - Trim spaces in string
extension String {
    var condensedWhitespace: String {
        let components = self.components(separatedBy: NSCharacterSet.whitespacesAndNewlines)
        return components.filter { !$0.isEmpty }.joined(separator: " ")
    }
}

extension String {
    var camelCased: String {
        guard !isEmpty else { return "" }
        return self
            .lowercased()
            .split(separator: "_")
            .map { String($0) }
            .enumerated()
            .map { $0.offset > 0 ? $0.element.capitalized : $0.element.lowercased() }
            .joined()
        
    }
}
