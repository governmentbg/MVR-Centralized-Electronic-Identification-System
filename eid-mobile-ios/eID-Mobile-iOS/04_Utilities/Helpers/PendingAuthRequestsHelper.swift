//
//  PendingAuthRequestsHelper.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 30.07.24.
//

import Foundation


final class PendingAuthRequestsHelper {
    // MARK: - Properties
    private init() { }
    
    private static var defaults: UserDefaults {
        return UserDefaults.standard
    }
    
    static func setTab(newTab: EIDTabItems) {
        defaults.set(newTab.rawValue, forKey: Constants.UserDefaultsKeys.selectedTab)
    }
    
    static func resetTab() {
        defaults.set(EIDTabItems.home.rawValue, forKey: Constants.UserDefaultsKeys.selectedTab)
    }
    
    // MARK: - API calls
    static func checkPendingAuthRequest() {
        ApprovalRequestRouter.getApprovalRequests.send(ApprovalRequestResponse.self) { response in
            switch response {
            case .success(let response):
                if response?.requests.isEmpty == false {
                    PendingAuthRequestsHelper.setTab(newTab: .pending)
                }
            case .failure(_):
                break
            }
        }
    }
}
