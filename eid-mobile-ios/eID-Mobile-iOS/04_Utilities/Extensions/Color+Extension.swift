//
//  Color+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.09.23.
//

import Foundation
import SwiftUI


extension Color {
    // Gradient
    static let gradientColors: [Color] = [.themeSecondaryLight, .themePrimaryLight, .themeSecondaryLight]
    
    var uiColor: UIColor {
        UIColor(self)
    }
}

extension Color {
    init?(hex: String) {
        var hexSanitized = hex.trimmingCharacters(in: .whitespacesAndNewlines)
        if hexSanitized.hasPrefix("#") {
            hexSanitized.removeFirst()
        }

        var rgb: UInt64 = 0
        guard Scanner(string: hexSanitized).scanHexInt64(&rgb) else {
            return nil
        }

        let r = Double((rgb >> 16) & 0xFF) / 255
        let g = Double((rgb >> 8) & 0xFF) / 255
        let b = Double(rgb & 0xFF) / 255

        self.init(red: r, green: g, blue: b)
    }
}
