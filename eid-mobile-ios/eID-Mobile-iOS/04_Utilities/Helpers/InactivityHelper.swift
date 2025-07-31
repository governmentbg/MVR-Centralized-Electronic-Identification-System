//
//  InactivityHelper.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.06.24.
//

import Foundation


final class InactivityHelper {
    // MARK: - Properties
    static let keyBackgroundTime = "enter_background_time"
    static let dateFormat = DateFormat.dayFullTime
    static var inactivityTimer: EIDTimer?
    static var type: InactivityHelperType = .normal
    
    // MARK: - Public methods
    static func startTimer() {
        killTimer()
        inactivityTimer = EIDTimer(Δt: Double(type.inactivityTimeLimitInSeconds), timerFinished: {
            sendLogoutNotification()
        })
    }
    
    static func saveBackgroundTime() {
        killTimer()
        UserDefaults.standard.set(Date().timeIntervalSince1970, forKey: keyBackgroundTime)
    }
    
    static func checkExpired() {
        if shouldLogout() {
            sendLogoutNotification()
        } else {
            startTimer()
        }
        removeBackgroundTime()
    }
    
    static func setType(newType: InactivityHelperType) {
        type = newType
        startTimer()
    }
    
    static func removeBackgroundTime() {
        UserDefaults.standard.removeObject(forKey: keyBackgroundTime)
    }
    
    // MARK: - Private methods
    static private func shouldLogout() -> Bool {
        let timeInterval = UserDefaults.standard.double(forKey: keyBackgroundTime)
        guard timeInterval != 0 else { return false }
        let difference = Int(Date().timeIntervalSince1970 - timeInterval)
        return difference >= type.inactivityTimeLimitInSeconds
    }
    
    static private func sendLogoutNotification() {
        killTimer()
        NotificationCenter.default.post(name: .inactivityLogout, object: nil)
    }
    
    static private func killTimer() {
        if inactivityTimer != nil {
            inactivityTimer?.kill()
        }
        removeBackgroundTime()
    }
}


enum InactivityHelperType {
    case normal
    case signing
    
    var inactivityTimeLimitInSeconds: Int {
        switch self {
        case .normal:
            120
        case .signing:
            300
        }
    }
}
