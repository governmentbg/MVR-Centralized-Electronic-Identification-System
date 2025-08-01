//
//  NoRepeatRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 11.06.25.
//

import Foundation


class ValidDevicePINRule: ValidationRule {
    // MARK: - Public methods
    func isValid(for string: String) -> ValidationResult {
        let repetition = findRepetition(string)
        let noConsecutiveDigits = noConsecutiveDigits(for: string)
        
        if (repetition.count == 0 || (repetition.count == 1 && repetition.string.count == 1)) && noConsecutiveDigits {
            return .valid
        } else {
            return ValidationResult(isValid: false, error: .createPinEasy)
        }
    }
    
    private func noConsecutiveDigits(for string: String) -> Bool {
        let array = Array(string).map { Int("\($0)")!}
        
        let output = stride(from: 0, to: array.count - 1, by: 1).map{(array[$0], array[$0 + 1])}
        let differences = output.map({ $0.1 - $0.0 })
        
        let countOne = differences.consecutiveAppearances(of: 1)
        let countMinusOne = differences.consecutiveAppearances(of: -1)
        
        return max(countOne, countMinusOne) < 3
    }
    
    private func findRepetition(_ s: String) -> (count: Int, string: String) {
        if s.isEmpty { return (0, "") }
        let pattern = "([0-9]+)\\1+"
        let regex = try? NSRegularExpression(pattern: pattern, options: [])
        let matches = regex?.matches(in: s, options: [], range: NSRange(location: 0, length: s.utf16.count)) ?? []
        var matchedString = ""
        if let unitRange = matches.first?.range(at: 1) {
            matchedString = (s as NSString).substring(with: unitRange)
        }
        
        return (matches.count, matchedString)
    }
}


extension Sequence where Element: Equatable {
    func consecutiveAppearances(of element: Element) -> Int {
        var occurance = 0
        
        for item in self {
            if item == element {
                occurance += 1
            } else if occurance > 0 {
                break
            }
        }
        
        if occurance == 1 {
            return 0
        }
        
        return occurance
    }
}
