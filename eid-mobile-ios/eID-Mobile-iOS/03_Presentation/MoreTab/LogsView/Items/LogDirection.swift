//
//  LogDirection.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 23.01.24.
//

import Foundation


enum LogDirection {
    case fromMe
    case toMe
    
    var title: String {
        switch self {
        case .fromMe:
            return "logs_from_me_title"
        case .toMe:
            return "logs_to_me_title"
        }
    }
    
    var subtitle: String {
        switch self {
        case .fromMe:
            return "logs_from_me_subtitle"
        case .toMe:
            return "logs_to_me_subtitle"
        }
    }
    
    var iconName: String {
        switch self {
        case .fromMe:
            return "icon_logs_from_me"
        case .toMe:
            return "icon_logs_to_me"
        }
    }
}
