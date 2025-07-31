//
//  IsAdultRule.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 5.06.24.
//

import Foundation


class IsAdultRule: ValidationRule {
    func isValid(for string: String) -> ValidationResult {
        let age = calculateAge(dob: string)
        return age >= 18 ? .valid : ValidationResult(isValid: false, error: .underaged)
    }
    
    func calculateAge(dob: String) -> Int {
        let dateFormater = DateFormatter()
        dateFormater.dateFormat = DateFormat.iso8601Date.rawValue
        let birthdayDate = dateFormater.date(from: dob)
        if let calendar: NSCalendar = NSCalendar(calendarIdentifier: .gregorian) {
            let now = Date()
            guard let birthdayDate = birthdayDate else { return 0 }
            let calcAge = calendar.components(.year, from: birthdayDate, to: now, options: [])
            let age = calcAge.year
            return age ?? 0
        }
        return 0
    }
}
