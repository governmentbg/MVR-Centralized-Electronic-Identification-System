//
//  Double+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation


extension Double {
    var decimalValue: Decimal {
        return NSNumber(floatLiteral: self).decimalValue
    }
    
    /// Rounds the double to decimal places value
    func rounded(toPlaces places:Int) -> Double {
        let divisor = pow(10.0, Double(places))
        return (self * divisor).rounded() / divisor
    }
}
