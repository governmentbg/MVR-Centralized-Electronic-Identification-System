//
//  ClearAlertOnLogoutModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 26.06.24.
//

import SwiftUI


struct ClearUIOnLogoutModifier: ViewModifier {
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .onReceive(NotificationCenter.default.publisher(for: .inactivityLogout), perform: { _ in
                if let vc = UIApplication.topMostViewController() {
                    let description = vc.description
                    var view: UIView?
                    switch description {
                    case _ where description.contains("SwiftUI.PlatformAlertController"):
                        if let alertView = vc.view.subviews.first {
                            view = alertView
                        }
                    case _ where description.contains("PresentationHostingController"):
                        if let popOverView = vc.presentationController?.presentedView {
                            view = popOverView
                        }
                    default:
                        break
                    }
                    view?.removeFromSuperview()
                }
            })
    }
}

extension View {
    func clearUI() -> some View {
        self.modifier(ClearUIOnLogoutModifier())
    }
}
