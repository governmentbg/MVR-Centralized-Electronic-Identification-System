//
//  CharacterSet+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 2.09.24.
//

import Foundation


extension CharacterSet {
    // MARK: - Init from range
    static func getCharsetFromUnicodeRange(_ range: ClosedRange<UInt32>) -> CharacterSet {
        return CharacterSet(charactersIn: String(String.UnicodeScalarView(range.compactMap(UnicodeScalar.init))))
    }
    
    // MARK: - Digits
    public static var digits: CharacterSet {
        let digitsRange = UInt32("0")...UInt32("9")
        return getCharsetFromUnicodeRange(digitsRange)
    }
    
    // MARK: - Latin Letters
    public static var uppercaseLatinLetters: CharacterSet {
        let uppercaseLettersRange = UInt32("A")...UInt32("Z")
        return getCharsetFromUnicodeRange(uppercaseLettersRange)
    }
    
    public static var lowercaseLatinLetters: CharacterSet {
        let lowercaseLettersRange = UInt32("a") ... UInt32("z")
        return getCharsetFromUnicodeRange(lowercaseLettersRange)
    }
    
    // MARK: - Bulgarian Cyrillic Letters
    public static var uppercaseBulgarianCyrillicLetters: CharacterSet {
        let uppercaseLettersRange = UInt32("А") ... UInt32("Я")
        let forbidenChars = CharacterSet(charactersIn: "эыЫЭ")
        return getCharsetFromUnicodeRange(uppercaseLettersRange).subtracting(forbidenChars)
    }
    
    public static var lowercaseBulgarianCyrillicLetters: CharacterSet {
        let lowercaseLettersRange = UInt32("а") ... UInt32("я")
        let forbidenChars = CharacterSet(charactersIn: "эыЫЭ")
        return getCharsetFromUnicodeRange(lowercaseLettersRange).subtracting(forbidenChars)
    }
}
