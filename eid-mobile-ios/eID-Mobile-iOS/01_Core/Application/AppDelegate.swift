//
//  AppDelegate.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 5.09.23.
//

import Foundation
import UIKit
import Firebase
import AlamofireNetworkActivityLogger


class AppDelegate: NSObject, UIApplicationDelegate {
    // MARK: - Application lifecycle
    func application(_ application: UIApplication,
                     didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey : Any]? = nil) -> Bool {
        setupFirebase()
        application.registerForRemoteNotifications()
        NetworkActivityLogger.shared.level = .debug
        NetworkActivityLogger.shared.startLogging()
        LanguageManager.initLanguage()
        
        if UserManager.hasUser {
            UserManager.removeUser()
        } else {
            InactivityHelper.removeBackgroundTime()
        }
        
        PendingAuthRequestsHelper.resetTab()
        AppConfiguration.initEnvironment()
        
        let fileName = "com.eID.iOS-\(Date().normalizeDate(outputFormat: .log)).log"
        if let path = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask).first?.appendingPathComponent(fileName) {
            DebugLogger.shared.redirectOutputToFile(path)
        }
        
#if DEBUG
//        UserManager.removeUserCredentials()
#endif
        
        return true
    }
    
    // MARK: - Firebase
    private func setupFirebase() {
        FirebaseApp.configure()
        Messaging.messaging().delegate = self
        UNUserNotificationCenter.current().delegate = self
        let authOptions: UNAuthorizationOptions = [.alert, .badge, .sound]
        UNUserNotificationCenter.current().requestAuthorization(options: authOptions,
                                                                completionHandler: { _, _ in })
    }
}


// MARK: - UNUserNotificationCenterDelegate
extension AppDelegate: UNUserNotificationCenterDelegate {
    // Receive displayed notifications for iOS 10 devices.
    func userNotificationCenter(_ center: UNUserNotificationCenter,
                                willPresent notification: UNNotification,
                                withCompletionHandler completionHandler: @escaping (UNNotificationPresentationOptions) -> Void) {
        let userInfo = notification.request.content.userInfo
        
        // With swizzling disabled you must let Messaging know about the message, for Analytics
        Messaging.messaging().appDidReceiveMessage(userInfo)
        // Print message ID.
        
        // Change this to your preferred presentation option
        completionHandler([.banner,.sound])
    }
    
    func userNotificationCenter(_ center: UNUserNotificationCenter,
                                didReceive response: UNNotificationResponse,
                                withCompletionHandler completionHandler: @escaping () -> Void) {
        let userInfo = response.notification.request.content.userInfo
        // Print message ID.
        // Print full message.
        print(userInfo)
        
        completionHandler()
    }
}


// MARK: - MessagingDelegate
extension AppDelegate: MessagingDelegate {
    // [START refresh_token]
    func messaging(_ messaging: Messaging, didReceiveRegistrationToken fcmToken: String?) {
        let dataDict: [String: String] = ["token": fcmToken ?? ""]
        NotificationCenter.default.post(name: Notification.Name("FCMToken"), object: nil, userInfo: dataDict)
        if let token = fcmToken {
            debugPrint("FCM Token received: \(token)")
            StorageManager.keychain.save(key: .firebaseToken, value: token)
        }
    }
    
    func application(_ application: UIApplication, didRegisterForRemoteNotificationsWithDeviceToken deviceToken: Data) {
        Messaging.messaging().setAPNSToken(deviceToken, type: .unknown)
    }
}
