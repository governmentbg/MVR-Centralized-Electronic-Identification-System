//
//  CustomWebView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 18.06.24.
//

import WebKit


class CustomWebView: WKWebView {
    // MARK: - Properties
    var shouldAuthenticate = false
    
    // MARK: - Override
    override func load(_ request: URLRequest) -> WKNavigation? {
        guard let mutableRequest: NSMutableURLRequest = request as? NSMutableURLRequest else {
            return super.load(request)
        }
        
        if shouldAuthenticate {
            if let bearerToken = StorageManager.keychain.getFor(key: .authToken) {
                let bearerToken = "Bearer \(bearerToken)"
                mutableRequest.setValue(bearerToken, forHTTPHeaderField: "Authorization")
            }
        }
        
        return super.load(mutableRequest as URLRequest)
    }
    
    // MARK: - Helpers
    func setInspectable() {
        perform(Selector(("setInspectable:")), with: true)
    }
}

