//
//  TransparentGradientDividerNavigationBarModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 11.07.24.
//

import SwiftUI


struct GradientDividerNavigationBarModifier: ViewModifier {
    // MARK: - Init
    init(backgroundColor: UIColor, tintColor: UIColor) {
        let coloredAppearance = UINavigationBarAppearance()
        coloredAppearance.configureWithOpaqueBackground()
        coloredAppearance.backgroundColor = backgroundColor
        coloredAppearance.titleTextAttributes = [.foregroundColor: tintColor,
                                                 .font: UIFont.heading4]
        coloredAppearance.largeTitleTextAttributes = [.foregroundColor: tintColor]
        coloredAppearance.shadowColor = .clear
        
        UINavigationBar.appearance().standardAppearance = coloredAppearance
        UINavigationBar.appearance().scrollEdgeAppearance = coloredAppearance
        UINavigationBar.appearance().compactAppearance = coloredAppearance
        UINavigationBar.appearance().tintColor = tintColor
    }
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .onViewWillAppear { vc in
                if let navVC = vc.navigationController {
                    let navBar = navVC.navigationBar
                    let height = navBar.frame.size.height
                    let frame = CGRect(origin: CGPoint(x: 0, y: height), size:
                                        CGSize(width: UIScreen.main.bounds.width, height: 1))
                    navBar.addSubview(HostingView(rootView: GradientDivider(), frame: frame))
                }
            }
    }
}
