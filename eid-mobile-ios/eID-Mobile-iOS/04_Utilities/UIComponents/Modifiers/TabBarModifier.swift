//
//  TabBarModifier.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 26.10.23.
//

import SwiftUI


struct TabBarModifier: ViewModifier {
    // MARK: - Init
    init(backgroundColor: UIColor, selectedColor: UIColor, unselectedColor: UIColor) {
        let itemAppearance = UITabBarItemAppearance()
        itemAppearance.normal.iconColor = unselectedColor
        itemAppearance.normal.titleTextAttributes = [.font: UIFont.bodySmall,
                                                     .foregroundColor: unselectedColor]
        itemAppearance.selected.iconColor = selectedColor
        itemAppearance.selected.titleTextAttributes = [.font: UIFont.bodySmall,
                                                       .foregroundColor: selectedColor]
        
        let tabBarAppearance = UITabBarAppearance()
        tabBarAppearance.backgroundColor = backgroundColor
        tabBarAppearance.stackedLayoutAppearance = itemAppearance
        tabBarAppearance.inlineLayoutAppearance = itemAppearance
        tabBarAppearance.compactInlineLayoutAppearance = itemAppearance
        
        UITabBar.appearance().standardAppearance = tabBarAppearance
        UITabBar.appearance().scrollEdgeAppearance = tabBarAppearance
    }
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
    }
}
