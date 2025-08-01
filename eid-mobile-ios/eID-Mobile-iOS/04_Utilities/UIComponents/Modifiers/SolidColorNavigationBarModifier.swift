//
//  SolidColorNavigationBarModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 25.10.23.
//

import SwiftUI


struct SolidColorNavigationBarModifier: ViewModifier {
    // MARK: - Init
    init(backgroundColor: UIColor, tintColor: UIColor) {
        let coloredAppearance = UINavigationBarAppearance()
        coloredAppearance.configureWithOpaqueBackground()
        coloredAppearance.backgroundColor = backgroundColor
        coloredAppearance.titleTextAttributes = [.foregroundColor: tintColor,
                                                 .font: UIFont.heading4]
        coloredAppearance.largeTitleTextAttributes = [.foregroundColor: tintColor]
        
        UINavigationBar.appearance().standardAppearance = coloredAppearance
        UINavigationBar.appearance().scrollEdgeAppearance = coloredAppearance
        UINavigationBar.appearance().compactAppearance = coloredAppearance
        UINavigationBar.appearance().tintColor = tintColor
        
        UILabel.appearance(whenContainedInInstancesOf: [UINavigationBar.self]).adjustsFontSizeToFitWidth = true
    }
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
    }
}
