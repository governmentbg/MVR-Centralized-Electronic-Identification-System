//
//  ScenePhaseModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.06.24.
//

import SwiftUI


struct ScenePhaseModifier: ViewModifier {
    // MARK: - Properties
    @Environment(\.scenePhase) var scenePhase
    var onActive: () -> ()
    var onBackground: () -> ()
    
    // MARK: - Body
    func body(content: Content) -> some View {
        content
            .onChange(of: scenePhase) { newPhase in
                if newPhase == .active {
                    onActive()
                } else if newPhase == .background {
                    onBackground()
                }
            }
    }
}

extension View {
    func observePhase(onActive: @escaping () -> (),
                      onBackground: @escaping () -> ()) -> some View {
        self.modifier(ScenePhaseModifier(onActive: onActive,
                                         onBackground: onBackground))
    }
}
