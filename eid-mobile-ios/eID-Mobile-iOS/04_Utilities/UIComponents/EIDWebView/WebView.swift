//
//  WebView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 18.06.24.
//

import SwiftUI
import WebKit


struct WebView: UIViewRepresentable {
    // MARK: - Properties
    @Binding var shouldRefresh: Bool
    var request: URLRequest?
    var shouldAuthenticate = false
    var navigationState: WebViewNavigationState
    
    // MARK: - UI
    func makeUIView(context: UIViewRepresentableContext<WebView>) -> WKWebView {
        let webview = navigationState.webView
        webview.shouldAuthenticate = shouldAuthenticate
        webview.isOpaque = false
        webview.navigationDelegate = navigationState
        webview.setInspectable()
        loadRequest()
        return webview
    }
    
    func updateUIView(_ uiView: WKWebView, context: UIViewRepresentableContext<WebView>) {
        if shouldRefresh {
            loadRequest()
        }
    }
    
    // MARK: - Helpers
    fileprivate func loadRequest() {
        if let request = request {
            let _ = navigationState.webView.load(request)
        }
    }
}
