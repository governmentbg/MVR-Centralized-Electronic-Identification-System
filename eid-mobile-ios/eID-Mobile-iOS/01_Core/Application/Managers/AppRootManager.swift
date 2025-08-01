//
//  AppRootManager.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation


final class AppRootManager: ObservableObject {
    // MARK: - Current root screen
    @Published var currentRoot: AppRoots = .login
    
    init() {
        observeServerResponses()
    }
    
    // MARK: - Root screen option
    enum AppRoots {
        case login
        case home
    }
    
    // MARK: - Helpers
    private func observeServerResponses() {
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(handleUnauthorizedRequest),
                                               name: .unauthorizedRequest,
                                               object: nil)
        
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(handleForceLogoutNotification),
                                               name: .inactivityLogout,
                                               object: nil)
    }
    
    @objc private func handleUnauthorizedRequest() {
        logoutUser()
    }
    
    @objc private func handleForceLogoutNotification() {
        logoutUser()
    }
    
    func logoutUser() {
        PendingAuthRequestsHelper.resetTab()
        UserManager.removeUser()
        UserManager.removeUserCredentials()
        InactivityHelper.removeBackgroundTime()
        currentRoot = .login
    }
}
