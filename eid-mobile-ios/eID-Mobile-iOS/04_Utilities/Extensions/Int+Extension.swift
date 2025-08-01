//
//  Int+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.04.25.
//

import Foundation

extension Int {
    var timeStringFor: String {
        let m: Int = (self/60) % 60
        let s: Int = self % 60
        let a = String(format: "%02u:%02u", m, s)
        return a
    }
}
