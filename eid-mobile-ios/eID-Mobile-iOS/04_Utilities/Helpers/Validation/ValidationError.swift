//
//  ValidationError.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 9.07.24.
//

import Foundation


enum ValidationError {
    // EGN
    case underaged
    case invalidEgn
    case invalidLnch
    case invalidEgnLength
    case invalidLnchLength
    case invalidEgnCharacters
    case invalidLnchCharacters
    case invalidSelfEmpowerment
    // EIK Bulstat
    case invalidEIKLength
    case invalidCharacters
    // Password
    case invalidPassword
    case passwordMismatch
    // PIN
    case invalidPin
    case pinMismatch
    case createPinEasy
    // Name
    case invalidName
    case invalidLatinName
    case firstCapitalLetter
    case invalidCompoundName
    // Email
    case invalidEmail
    // Zip code
    case invalidZipCode
    // ID Document number
    case invalidIdDocumentNumber
    case invalidNewIdDocumentNumber
    // Common
    case invalidDigits
    case invalidLength(length: Int)
    case invalidMinimumLength(length: Int)
    case emptyString
    case invalidStringRepeatValue
    case noError
}

extension ValidationError {
    var description: String {
        switch self {
        case .underaged:
            return "validation_error_underaged"
        case .invalidEgn:
            return "validation_error_invalid_eng"
        case .invalidLnch:
            return "validation_error_invalid_lnch"
        case .invalidEgnLength:
            return "validation_error_invalid_eng_length"
        case .invalidLnchLength:
            return "validation_error_invalid_lnch_length"
        case .invalidEgnCharacters:
            return "validation_error_invalid_eng_characters"
        case .invalidLnchCharacters:
            return "validation_error_invalid_lnch_characters"
        case .invalidSelfEmpowerment:
            return "validation_error_invalid_self_empowerment"
        case .invalidEIKLength:
            return "validation_error_invalid_eik_bulstat_length"
        case .invalidCharacters:
            return "validation_error_invalid_eik_bulstat_characters"
        case .invalidPassword:
            return "validation_error_invalid_password"
        case .passwordMismatch:
            return "validation_error_password_mismatch"
        case .invalidPin:
            return "validation_error_invalid_pin"
        case .pinMismatch:
            return "validation_error_pin_mismatch"
        case .invalidName:
            return "validation_error_invalid_name"
        case .invalidLatinName:
            return "validation_error_invalid_latin_name"
        case .firstCapitalLetter:
            return "validation_error_first_capital_letter"
        case .invalidCompoundName:
            return "validation_error_invalid_compound_name"
        case .invalidEmail:
            return "validation_error_invalid_email"
        case .invalidZipCode:
            return "validation_error_invalid_zip_code"
        case .invalidIdDocumentNumber:
            return "validation_error_invalid_id_document_number"
        case .invalidNewIdDocumentNumber:
            return "validation_error_invalid_new_id_document_number"
        case .invalidDigits:
            return "validation_error_invalid_digits"
        case .invalidLength(let length):
            return String(format: "validation_error_invalid_length".localized(), length)
        case .invalidMinimumLength(let length):
            return String(format: "validation_error_invalid_minimum_length".localized(), length)
        case .emptyString:
            return "validation_error_empty_string"
        case .invalidStringRepeatValue:
            return "validation_error_invalid_repeated_string"
        case .createPinEasy:
            return "validation_error_device_pin_easy"
        case .noError:
            return ""
        }
    }
}
