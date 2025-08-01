//
//  ValidationProtocol.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 24.06.24.
//

import SwiftUI

protocol Validation {
    var validation: ValidationResult { get set }
    func validate() -> ValidationResult
}

extension Validation {
    var validation: ValidationResult {
        get {
            return validate()
        }
        set {}
    }
}
