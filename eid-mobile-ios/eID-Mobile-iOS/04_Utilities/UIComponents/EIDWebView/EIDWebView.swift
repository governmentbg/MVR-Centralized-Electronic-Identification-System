//
//  EIDWebView.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 18.06.24.
//

import SwiftUI


// MARK: - Webview type enum
enum EIDWebViewType {
    case home
    case faq
    case contacts
    case termsAndConditions
    case administrators
    case centers
    case providers
    case secureDeliverySystem
    case onlineHelpSystem
    
    var url: String {
        guard let baseURL = AppConfiguration.get(keychainKey: ServiceDomain.PG.baseUrl) as? String else {
            return ""
        }
        let currentLanguage = LanguageManager.preferredLanguage ?? .bg
        var urlToLoad: String = ""
        
        switch self {
        case .home:
            urlToLoad = "\(baseURL)mobile/home"
        case .faq:
            urlToLoad = "\(baseURL)mobile/useful-information"
        case .contacts:
            urlToLoad = "\(baseURL)mobile/contacts"
        case .termsAndConditions:
            urlToLoad = "\(baseURL)mobile/terms-and-conditions"
        case .administrators:
            urlToLoad = "\(baseURL)mobile/home/administrators"
        case .centers:
            urlToLoad = "\(baseURL)mobile/home/centers"
        case .providers:
            urlToLoad = "\(baseURL)mobile/home/providers"
        case .secureDeliverySystem:
            return "https://edelivery.egov.bg/"
        case .onlineHelpSystem:
            return OnlineHelpSystemLinkByEnvironment.link
        }
        return "\(urlToLoad)?lang=\(currentLanguage.rawValue)"
    }
}

struct EIDWebView: View {
    // MARK: - Properties
    var type: EIDWebViewType
    @State var shouldAuthenticate: Bool
    // MARK: - Private Properties
    @State private var request: URLRequest?
    @State private var shouldRefresh = false
    @StateObject private var navigationState = WebViewNavigationState()
    
    init(type: EIDWebViewType, shouldAuthenticate: Bool? = nil) {
        self.type = type
        self.shouldAuthenticate = shouldAuthenticate ?? false
    }
    
    // MARK: - Body
    var body: some View {
        WebView(shouldRefresh: $shouldRefresh,
                request: request,
                shouldAuthenticate: shouldAuthenticate,
                navigationState: navigationState)
        .onAppear {
            loadUrl()
        }
    }
    
    // MARK: - Helpers
    private func loadUrl() {
        if let url = URL(string: type.url) {
            request = URLRequest(url: url)
            shouldRefresh = true
        }
    }
}

#Preview {
    EIDWebView(type: .contacts)
}
