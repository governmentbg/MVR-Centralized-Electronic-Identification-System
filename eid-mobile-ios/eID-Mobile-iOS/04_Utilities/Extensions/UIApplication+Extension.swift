//
//  UIApplication+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 3.06.24.
//

import SwiftUI


extension Notification.Name {
    // custom notification when user activity is detected
    static let userActivityDetected = Notification.Name("UserActivityDetected")
}

extension UIApplication {
    // names the gesture recognizer so that it can be removed later on
    static let userActivityGestureRecognizer = "userActivityGestureRecognizer"
    
    // Returns `true` if user activity tracker is registered on the app.
    var hasUserActivityTracker: Bool {
        keyWindow?.gestureRecognizers?.contains(where: { $0.name == UIApplication.userActivityGestureRecognizer }) == true
    }
    
    // Adds a tap gesture recognizer to intercept any touches, while still
    // propagating interactions to UI elements.
    func addUserActivityTracker() {
        guard let window = keyWindow else { return }
        let gesture = UITapGestureRecognizer(target: window, action: nil)
        gesture.requiresExclusiveTouchType = false
        gesture.cancelsTouchesInView = false
        gesture.delegate = self
        gesture.name = UIApplication.userActivityGestureRecognizer
        window.addGestureRecognizer(gesture)
    }
    
    // Removes the tap gesture recognizer that detects user interactions.
    func removeUserActivityTracker() {
        guard let window = keyWindow,
              let gesture = window.gestureRecognizers?.first(where: { $0.name == UIApplication.userActivityGestureRecognizer })
        else {
            return
        }
        window.removeGestureRecognizer(gesture)
    }
    
    var keyWindow: UIWindow? {
        let allScenes = UIApplication.shared.connectedScenes
        for scene in allScenes {
            guard let windowScene = scene as? UIWindowScene else { continue }
            for window in windowScene.windows where window.isKeyWindow {
                return window
            }
        }
        return nil
    }
}

extension UIApplication: @retroactive UIGestureRecognizerDelegate {
    public func gestureRecognizer(_ gestureRecognizer: UIGestureRecognizer,
                                  shouldReceive touch: UITouch) -> Bool {
        NotificationCenter.default.post(name: .userActivityDetected, object: nil)
        return true
    }
    
    public func gestureRecognizer(_ gestureRecognizer: UIGestureRecognizer,
                                  shouldRecognizeSimultaneouslyWith otherGestureRecognizer: UIGestureRecognizer) -> Bool {
        true
    }
}

extension UIApplication {
    var currentWindow: UIWindow? {
        connectedScenes
            .filter({$0.activationState == .foregroundActive})
            .map({$0 as? UIWindowScene})
            .compactMap({$0})
            .first?.windows
            .filter({$0.isKeyWindow}).first
    }
    
    class func topMostViewController(base: UIViewController? = UIApplication.shared.currentWindow?.rootViewController) -> UIViewController? {
        if let nav = base as? UINavigationController {
            return topMostViewController(base: nav.visibleViewController)
        }
        if let tab = base as? UITabBarController {
            let moreNavigationController = tab.moreNavigationController
            if let top = moreNavigationController.topViewController, 
                top.view.window != nil {
                return topMostViewController(base: top)
            } else if let selected = tab.selectedViewController {
                return topMostViewController(base: selected)
            }
        }
        if let presented = base?.presentedViewController {
            return topMostViewController(base: presented)
        }
        return base
    }
}
