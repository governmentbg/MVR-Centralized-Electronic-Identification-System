//
//  WebViewNavigationState.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 18.06.24.
//

import SwiftUI
@preconcurrency import WebKit


class WebViewNavigationState: NSObject, ObservableObject {
    let webView = CustomWebView()
    
    private func openLink(url: URL) {
        if UIApplication.shared.canOpenURL(url) {
            UIApplication.shared.open(url)
        }
    }
}

extension WebViewNavigationState: WKNavigationDelegate {
    func webView(_ webView: WKWebView,
                 decidePolicyFor navigationAction: WKNavigationAction,
                 preferences: WKWebpagePreferences,
                 decisionHandler: @escaping (WKNavigationActionPolicy, WKWebpagePreferences) -> Void) {
        guard let scheme = navigationAction.request.url?.scheme,
              let url = navigationAction.request.url else {
            decisionHandler(.allow, preferences)
            return
        }
        
        switch scheme {
        case "tel", "mailto":
            openLink(url: url)
            decisionHandler(.cancel, preferences)
        default:
            switch navigationAction.navigationType {
            case .linkActivated:
                if let url = navigationAction.request.url {
                    openLink(url: url)
                }
                
                decisionHandler(.cancel, preferences)
            default:
                decisionHandler(.allow, preferences)
            }
        }
    }
    
    func webView(_ webView: WKWebView, didReceive challenge: URLAuthenticationChallenge, completionHandler: @escaping (URLSession.AuthChallengeDisposition, URLCredential?) -> Void) {
        guard let serverTrust = challenge.protectionSpace.serverTrust else { return completionHandler(.useCredential, nil) }
        let exceptions = SecTrustCopyExceptions(serverTrust)
        SecTrustSetExceptions(serverTrust, exceptions)
        completionHandler(.useCredential, URLCredential(trust: serverTrust))
    }
}
