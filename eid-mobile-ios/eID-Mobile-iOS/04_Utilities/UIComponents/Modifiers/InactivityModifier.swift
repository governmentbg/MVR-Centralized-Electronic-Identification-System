//
//  InactivityModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.06.24.
//

import SwiftUI


struct InactivityModifier: ViewModifier {
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .observePhase(onActive: {
                if UserManager.hasUser {
                    InactivityHelper.checkExpired()
                }
            }, onBackground: {
                if UserManager.hasUser {
                    InactivityHelper.saveBackgroundTime()
                }
            })
            .onReceive(NotificationCenter.default.publisher(for: .userActivityDetected), perform: { _ in
                if UserManager.hasUser {
                    InactivityHelper.startTimer()
                }
            })
            .onAppear(perform: UIApplication.shared.addUserActivityTracker)
            .onDisappear(perform: UIApplication.shared.removeUserActivityTracker)
    }
}

extension View {
    func observeInactivity() -> some View {
        self.modifier(InactivityModifier())
    }
}
