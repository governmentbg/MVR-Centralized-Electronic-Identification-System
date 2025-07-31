//
//  eID_Mobile_iOSApp.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 31.08.23.
//

import SwiftUI
import Firebase


@main
struct eID_Mobile_iOSApp: App {
    // MARK: - Properties
    // Register app delegate for Firebase setup
    @UIApplicationDelegateAdaptor(AppDelegate.self) var delegate
    @StateObject private var appRootManager = AppRootManager()
    @StateObject var networkMonitor = NetworkMonitor.shared
    @State var showLoading = false
    
    // MARK: - Body
    var body: some Scene {
        WindowGroup {
            Group {
                switch appRootManager.currentRoot {
                case .login:
                    MainLoginView()
                case .home:
                    MainTabView()
                }
            }
            .onAppear {
                showLoading = true
                GlobalLocalisations.getServerSettings()
                GlobalLocalisations.didFinishLoading = {
                    showLoading = false
                }
            }
            .observeLoading(isLoading: $showLoading)
            .environmentObject(appRootManager)
            .environmentObject(networkMonitor)
            .overlay {
                if !networkMonitor.isConnected {
                    NoInternetView()
                }
            }
            .observeInactivity()
            .clearUI()
        }
    }
}
